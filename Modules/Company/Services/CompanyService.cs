using MongoDB.Bson;
using MongoDB.Driver;
using seguimiento_expotec.Connection;
using seguimiento_expotec.Modules.BusinessExecutives.Services;
using seguimiento_expotec.Modules.Company.Persistence.DTO;
using seguimiento_expotec.Modules.Company.Persistence.Models;
using seguimiento_expotec.Modules.Company.Services;

namespace seguimiento_expotec.Modules.Company.Services
{
    public class CompanyService
    {
        private readonly ConnectionDB connection;
        private readonly BusinessExecutiveService businessExecutiveService;

        public CompanyService(ConnectionDB connection, BusinessExecutiveService businessExecutiveService)
        {
            this.connection = connection;
            this.businessExecutiveService = businessExecutiveService;
        }

        private CompanyDTO ConvertToDTO(CompanyModel company)
        {
            return new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name,
                Ruc = company.Ruc,
                Address = company.Address,
                District = company.District,
                Region = company.Region,
                ExecutiveId = company.ExecutiveId?.ToString()
            };
        }

        public async Task<List<CompanyDTO>> GetAllCompanies()
        {
            try
            {
                var col = connection.Connect().GetCollection<CompanyModel>("company");
                var result = await col.Find(FilterDefinition<CompanyModel>.Empty).ToListAsync();
                var companyDTOs = result.Select(c => ConvertToDTO(c)).ToList();
                return companyDTOs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las compañías: {ex.Message}");
                throw;
            }
        }

        public async Task<CompanyModel?> GetCompanyByIdOrRuc(string idOrRuc)
        {
            try
            {
                var col = connection.Connect().GetCollection<CompanyModel>("company");
                var isHexId = ObjectId.TryParse(idOrRuc, out _);
                var filter = isHexId ? Builders<CompanyModel>.Filter.Eq(c => c.Id, idOrRuc) : Builders<CompanyModel>.Filter.Eq(c => c.Ruc, idOrRuc);
                var result = await col.Find(filter).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la compañía: {ex.Message}");
                throw;
            }
        }

        public async Task<object> CreateCompany(CompanyDTO company)
        {
            try
            {
                if (string.IsNullOrEmpty(company.Ruc))
                {
                    throw new ArgumentException("El RUC no puede ser nulo o vacío.");
                }

                var existingCompany = await GetCompanyByIdOrRuc(company.Ruc);
                if (existingCompany != null)
                {
                    return new { Error = $"Ya existe una compañía con el RUC {company.Ruc}. No se creó la nueva compañía." };
                }

                if (!string.IsNullOrEmpty(company.ExecutiveId))
                {
                    var executive = await businessExecutiveService.GetBusinessExecutiveById(company.ExecutiveId);
                    if (executive == null)
                    {
                        return new { Error = $"No existe un ejecutivo con el ID {company.ExecutiveId}." };
                    }
                }

                var companyModel = new CompanyModel
                {
                    Name = company.Name,
                    Ruc = company.Ruc,
                    Address = company.Address,
                    District = company.District,
                    Region = company.Region,
                    ExecutiveId = !string.IsNullOrEmpty(company.ExecutiveId) ? ObjectId.Parse(company.ExecutiveId) : (ObjectId?)null
                };

                var col = connection.Connect().GetCollection<CompanyModel>("company");
                await col.InsertOneAsync(companyModel);

                // Comprobar si ExecutiveId es nulo antes de buscar el ejecutivo
                if (companyModel.ExecutiveId.HasValue)
                {
                    var executive = await businessExecutiveService.GetBusinessExecutiveById(companyModel.ExecutiveId.Value.ToString());
                    if (executive != null)
                    {
                        return new
                        {
                            Company = companyModel,
                            ExecutiveId = executive.Id,
                            ExecutiveName = executive.Name
                        };
                    }
                }

                return companyModel;
            }
            catch (Exception ex)
            {
                return new { Error = $"Error al insertar compañía: {ex.Message}" };
            }
        }



        public async Task<object> UpdateCompany(string id, CompanyDTO updatedCompany)
        {
            try
            {
                if (string.IsNullOrEmpty(updatedCompany.Ruc))
                {
                    throw new ArgumentException("El RUC no puede ser nulo o vacío.");
                }

                // Obtiene la compañía existente usando el método proporcionado
                var existingCompanyById = await GetCompanyByIdOrRuc(id);
                if (existingCompanyById == null)
                {
                    return new { Error = "La compañía con este ID no existe." };
                }

                // Verifica si el RUC ya existe en otra compañía
                if (existingCompanyById.Ruc != updatedCompany.Ruc)
                {
                    var existingCompanyByRuc = await GetCompanyByIdOrRuc(updatedCompany.Ruc);
                    if (existingCompanyByRuc != null)
                    {
                        return new { Message = "Ya existe una compañía con este RUC. No se realizó ninguna actualización." };
                    }
                }

                // Actualización del modelo existente para guardar en la base de datos
                var updatedModel = new CompanyModel
                {
                    Id = existingCompanyById.Id,
                    Name = updatedCompany.Name,
                    Ruc = updatedCompany.Ruc,
                    Address = updatedCompany.Address,
                    District = updatedCompany.District,
                    Region = updatedCompany.Region,
                    ExecutiveId = !string.IsNullOrEmpty(updatedCompany.ExecutiveId)
                        ? MongoDB.Bson.ObjectId.Parse(updatedCompany.ExecutiveId)
                        : (MongoDB.Bson.ObjectId?)null
                };

                // Conecta con la colección y actualiza
                var col = connection.Connect().GetCollection<CompanyModel>("company");
                var filter = Builders<CompanyModel>.Filter.Eq(c => c.Id, updatedModel.Id);
                var updateDefinition = Builders<CompanyModel>.Update
                    .Set(c => c.Name, updatedModel.Name)
                    .Set(c => c.Ruc, updatedModel.Ruc)
                    .Set(c => c.Address, updatedModel.Address)
                    .Set(c => c.District, updatedModel.District)
                    .Set(c => c.Region, updatedModel.Region)
                    .Set(c => c.ExecutiveId, updatedModel.ExecutiveId);

                var result = await col.UpdateOneAsync(filter, updateDefinition);

                if (result.ModifiedCount == 0)
                {
                    return new { Error = "No se pudo actualizar la compañía." };
                }

                // Devuelve el DTO actualizado para la respuesta
                return new
                {
                    Id = updatedModel.Id?.ToString() ?? string.Empty,
                    updatedCompany.Name,
                    updatedCompany.Ruc,
                    updatedCompany.Address,
                    updatedCompany.District,
                    updatedCompany.Region,
                    ExecutiveId = updatedModel.ExecutiveId?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new { Error = $"Error al actualizar compañía: {ex.Message}" };
            }
        }


        public async Task<object> DeleteCompany(string id)
        {
            try
            {
                var company = await GetCompanyByIdOrRuc(id);
                if (company == null)
                {
                    return new { Error = "No se encontró la compañía." };
                }
                var col = connection.Connect().GetCollection<CompanyDTO>("company");
                var result = await col.DeleteOneAsync(c => c.Id == company.Id);
                if (result.DeletedCount > 0)
                {
                    return new { DeletedCompany = company };
                }
                return new { Error = "No se pudo eliminar la compañía." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la compañía: {ex.Message}");
                return new { Error = $"Error al eliminar la compañía: {ex.Message}" };
            }
        }
    }
}
