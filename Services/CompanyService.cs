using MongoDB.Bson;
using MongoDB.Driver;
using seguimiento_expotec.Connection;
using seguimiento_expotec.Models;

namespace seguimiento_expotec.Services
{
    public class CompanyService
    {
        private readonly ConnectionDB connection;

        public CompanyService(ConnectionDB connection)
        {
            this.connection = connection;
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            var col = connection.Connect().GetCollection<Company>("company");
            var result = await col.Find(FilterDefinition<Company>.Empty).ToListAsync();
            return result;
        }

        public async Task<Company> GetCompanyByIdOrRuc(string idOrRuc)
        {
            try
            {
                var col = connection.Connect().GetCollection<Company>("company");
                var isHexId = ObjectId.TryParse(idOrRuc, out _);
                var filter = isHexId
                    ? Builders<Company>.Filter.Eq(c => c.Id, idOrRuc)
                    : Builders<Company>.Filter.Eq(c => c.Ruc, idOrRuc);
                var result = await col.Find(filter).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la compañía: {ex.Message}");
                throw;
            }
        }

        public async Task<object> CreateCompany(Company company)
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
                    return new { Message = $"Ya existe una compañía con el RUC {company.Ruc}. No se creó la nueva compañía." };
                }
                var col = connection.Connect().GetCollection<Company>("company");
                await col.InsertOneAsync(company);
                return company;
            }
            catch (Exception ex)
            {
                return new { Error = $"Error al insertar compañía: {ex.Message}" };
            }
        }


        public async Task<object> UpdateCompany(string id, Company updatedCompany)
        {
            try
            {
                if (string.IsNullOrEmpty(updatedCompany.Ruc))
                {
                    throw new ArgumentException("El RUC no puede ser nulo o vacío.");
                }
                var existingCompanyById = await GetCompanyByIdOrRuc(id);
                if (existingCompanyById == null)
                {
                    return new { Error = "La compañía con este ID no existe." };
                }
                if (existingCompanyById.Ruc != updatedCompany.Ruc)
                {
                    var existingCompanyByRuc = await GetCompanyByIdOrRuc(updatedCompany.Ruc);
                    if (existingCompanyByRuc != null)
                    {
                        return new { Message = "Ya existe una compañía con este RUC. No se realizó ninguna actualización." };
                    }
                }
                var col = connection.Connect().GetCollection<Company>("company");
                var filter = Builders<Company>.Filter.Eq(c => c.Id, id);
                var updateDefinition = Builders<Company>.Update
                    .Set(c => c.Name, updatedCompany.Name)
                    .Set(c => c.Ruc, updatedCompany.Ruc)
                    .Set(c => c.Address, updatedCompany.Address)
                    .Set(c => c.District, updatedCompany.District)
                    .Set(c => c.Region, updatedCompany.Region);
                var result = await col.UpdateOneAsync(filter, updateDefinition);
                if (result.ModifiedCount == 0)
                {
                    return new { Error = "No se pudo actualizar la compañía." };
                }
                return updatedCompany;
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
                var col = connection.Connect().GetCollection<Company>("company");
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
