using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;
using seguimiento_expotec.Connection;
using seguimiento_expotec.Modules.BusinessExecutives.Persistence.DTO;
using seguimiento_expotec.Modules.BusinessExecutives.Persistence.Models;

namespace seguimiento_expotec.Modules.BusinessExecutives.Services
{
    public class BusinessExecutiveService
    {
        private readonly ConnectionDB connection;

        public BusinessExecutiveService(ConnectionDB connection)
        {
            this.connection = connection;
        }

        private BusinessExecutivesDTO ConvertToDTO(BusinessExecutivesModel executive)
        {
            return new BusinessExecutivesDTO
            {
                Id = executive.Id,
                Name = executive.Name
            };
        }

        public async Task<List<BusinessExecutivesModel>> GetAllBusinessExecutives()
        {
            var col = connection.Connect().GetCollection<BusinessExecutivesModel>("businessExecutives");
            var result = await col.Find(FilterDefinition<BusinessExecutivesModel>.Empty).ToListAsync();
            return result;
        }

        public async Task<BusinessExecutivesDTO> GetBusinessExecutiveById(string id)
        {
            try
            {
                var col = connection.Connect().GetCollection<BusinessExecutivesModel>("businessExecutives");
                var filter = Builders<BusinessExecutivesModel>.Filter.Eq(be => be.Id, id);
                var result = await col.Find(filter).FirstOrDefaultAsync();
                if (result == null)
                {
                    throw new KeyNotFoundException($"No se encontró un ejecutivo con el ID '{id}'. Verifique que el ID sea correcto.");
                }
                return ConvertToDTO(result);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"No se encontró un ejecutivo con el ID '{id}'. Verifique que el ID sea correcto.", ex);
                throw;
            }
        }

        public async Task<BusinessExecutivesDTO> CreateBusinessExecutive(BusinessExecutivesDTO executiveDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(executiveDTO.Name))
                {
                    throw new ArgumentException("El nombre no puede ser nulo o vacío.");
                }
                var executiveModel = new BusinessExecutivesModel
                {
                    Name = executiveDTO.Name
                };
                var col = connection.Connect().GetCollection<BusinessExecutivesModel>("businessExecutives");
                await col.InsertOneAsync(executiveModel);
                return ConvertToDTO(executiveModel);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al insertar BusinessExecutive: {ex.Message}");
            }
        }

        public async Task<BusinessExecutivesDTO> UpdateBusinessExecutive(string id, BusinessExecutivesDTO updatedExecutiveDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(updatedExecutiveDTO.Name))
                {
                    throw new ArgumentException("El nombre no puede ser nulo o vacío.");
                }
                var currentExecutiveDTO = await GetBusinessExecutiveById(id);
                var updatedExecutiveModel = new BusinessExecutivesModel
                {
                    Id = id,
                    Name = updatedExecutiveDTO.Name
                };
                var col = connection.Connect().GetCollection<BusinessExecutivesModel>("businessExecutives");
                var filter = Builders<BusinessExecutivesModel>.Filter.Eq(be => be.Id, id);
                var update = Builders<BusinessExecutivesModel>.Update.Set(be => be.Name, updatedExecutiveModel.Name);
                var result = await col.UpdateOneAsync(filter, update);
                if (result.ModifiedCount == 0)
                {
                    throw new KeyNotFoundException($"No se encontró un ejecutivo con el id: {id} para actualizar.");
                }
                var updatedExecutive = await col.Find(filter).FirstOrDefaultAsync();
                return updatedExecutive != null ? ConvertToDTO(updatedExecutive)
                                                 : throw new InvalidOperationException("Error al recuperar el ejecutivo actualizado.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar BusinessExecutive: {ex.Message}", ex);
            }
        }

        public async Task<BusinessExecutivesDTO> DeleteBusinessExecutive(string id)
        {
            try
            {
                var executiveDTO = await GetBusinessExecutiveById(id);
                var col = connection.Connect().GetCollection<BusinessExecutivesModel>("businessExecutives");
                var filter = Builders<BusinessExecutivesModel>.Filter.Eq(be => be.Id, id);
                var result = await col.DeleteOneAsync(filter);
                if (result.DeletedCount == 0)
                {
                    throw new InvalidOperationException($"No se pudo eliminar el ejecutivo con el id: {id}.");
                }
                return executiveDTO;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar BusinessExecutive: {ex.Message}", ex);
            }
        }
    }
}
