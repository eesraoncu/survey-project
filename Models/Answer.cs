using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Answer
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("answers_text")]
    public string AnswersText { get; set; } = string.Empty;

    [BsonElement("questions_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int QuestionsId { get; set; } // Foreign Key

    [BsonElement("users_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UsersId { get; set; } // Foreign Key - Who answered

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveysId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 