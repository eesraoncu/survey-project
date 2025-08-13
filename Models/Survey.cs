using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("survey_name")]
    public string SurveyName { get; set; } = string.Empty;

    [BsonElement("survey_description")]
    public string SurveyDescription { get; set; } = string.Empty;

    [BsonElement("users_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UsersId { get; set; } // Foreign Key - Survey creator (will become owner)

    [BsonElement("survey_type_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveyTypeId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("is_completed")]
    public bool IsCompleted { get; set; } = false; // Survey completion status
} 