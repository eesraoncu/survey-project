using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("questions_text")]
    public string QuestionsText { get; set; } = string.Empty;

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SurveysId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 