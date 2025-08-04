using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("survey_name")]
    public string SurveyName { get; set; } = string.Empty;

    [BsonElement("survey_description")]
    public string SurveyDescription { get; set; } = string.Empty;

    [BsonElement("users_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UsersId { get; set; } = string.Empty; // Foreign Key - Survey creator

    [BsonElement("survey_type_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SurveyTypeId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
} 