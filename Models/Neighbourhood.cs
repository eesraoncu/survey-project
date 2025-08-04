using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Neighbourhood
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("neighbourhood_name")]
    public string NeighbourhoodName { get; set; } = string.Empty;

    [BsonElement("neighbourhood_explanation")]
    public string NeighbourhoodExplanation { get; set; } = string.Empty;

    [BsonElement("district_township_town_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string DistrictTownshipTownId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 