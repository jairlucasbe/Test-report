using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace seguimiento_expotec.Modules.Company.Persistence.DTO
{
    public class CompanyDTO
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("ruc")]
        public string? Ruc { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }

        [BsonElement("district")]
        public string? District { get; set; }

        [BsonElement("region")]
        public string? Region { get; set; }

        [BsonElement("executiveId")]
        public string? ExecutiveId { get; set; }
    }
}
