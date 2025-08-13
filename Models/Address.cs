using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Address
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("neighbourhood_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int NeighbourhoodId { get; set; } // Foreign Key

    [BsonElement("address_details")]
    public string AddressDetails { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 