using MongoDB.Bson.Serialization.Attributes;

namespace seguimiento_expotec.Modules.BusinessExecutives.Persistence.DTO
{
    public class BusinessExecutivesDTO
    {
        [BsonElement("_id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }
    }
}
