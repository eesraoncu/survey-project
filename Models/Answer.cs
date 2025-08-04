using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Answer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("answers_text")]
    public string AnswersText { get; set; } = string.Empty;

    [BsonElement("questions_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuestionsId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("users_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UsersId { get; set; } = string.Empty; // Foreign Key - Who answered

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SurveysId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 