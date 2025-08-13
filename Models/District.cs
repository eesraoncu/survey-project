using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class District
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("district_name")]
    public string DistrictName { get; set; } = string.Empty;

    [BsonElement("city_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int CityId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 