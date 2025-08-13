using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class DistrictTownshipTown
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("district_township_town_name")]
    public string DistrictTownshipTownName { get; set; } = string.Empty;

    [BsonElement("district_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int DistrictId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 