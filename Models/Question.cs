using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("questions_text")]
    public string QuestionsText { get; set; } = string.Empty;

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveysId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 