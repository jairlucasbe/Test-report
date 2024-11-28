using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace seguimiento_expotec.Modules.Guests.Persistence.Models
{
    public class Guests
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("dni")]
        public string? Dni { get; set; }

        [BsonElement("position")]
        public string? Position { get; set; }

        [BsonElement("area")]
        public string? Area { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("invitationDate")]
        public DateTime? InvitationDate { get; set; }

        [BsonElement("isCocktailCandidate")]
        public bool IsCocktailCandidate { get; set; } = false;

        [BsonElement("phoneNumbers")]
        public List<string> Phone_Numbers { get; set; } = new List<string>();

        [BsonElement("companyId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? CompanyId { get; set; }
    }
}
