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
    public string QuestionType { get; set; } = string.Empty; // Will store question type code

    [BsonElement("question_type_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int QuestionTypeId { get; set; } = 0; // Foreign Key to QuestionType collection

    [BsonElement("is_required")]
    public bool IsRequired { get; set; } = false;

    [BsonElement("order")]
    public int Order { get; set; } = 0; // Soru sırası

    [BsonElement("choices")]
    public List<string> Choices { get; set; } = new List<string>(); // Backward compatibility

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveysId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Helper properties - bunları controller'da QuestionType lookup ile dolduracağız
    [BsonIgnore]
    public bool RequiresChoices { get; set; } = false;

    [BsonIgnore]
    public string TypeDisplayName { get; set; } = string.Empty;

    [BsonIgnore]
    public bool AllowsMultipleSelection { get; set; } = false;
} 