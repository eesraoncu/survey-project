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

    [BsonElement("question_type")]
    public string QuestionType { get; set; } = string.Empty; // rating, multiple_choice, text, yes_no

    [BsonElement("choices")]
    public List<string> Choices { get; set; } = new List<string>(); // Soru seçenekleri

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveysId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 