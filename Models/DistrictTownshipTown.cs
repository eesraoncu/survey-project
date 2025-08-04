using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class DistrictTownshipTown
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("district_township_town_name")]
    public string DistrictTownshipTownName { get; set; } = string.Empty;

    [BsonElement("district_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string DistrictId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 