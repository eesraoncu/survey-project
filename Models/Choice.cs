using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Choice
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("choice_text")]
    public string ChoiceText { get; set; } = string.Empty;

    [BsonElement("questions_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int QuestionsId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 