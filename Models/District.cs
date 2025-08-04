using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class District
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("district_name")]
    public string DistrictName { get; set; } = string.Empty;

    [BsonElement("city_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CityId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 