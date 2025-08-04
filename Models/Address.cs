using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Address
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("neighbourhood_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string NeighbourhoodId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("address_details")]
    public string AddressDetails { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 