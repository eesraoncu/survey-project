using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class SmartQuestion
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("question_text")]
    public string QuestionText { get; set; } = string.Empty;

    [BsonElement("question_type")]
    public string QuestionType { get; set; } = string.Empty; // "multiple_choice", "text", "rating", "yes_no"

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("suggested_choices")]
    public List<string> SuggestedChoices { get; set; } = new List<string>();

    [BsonElement("ai_confidence")]
    public double AIConfidence { get; set; }

    [BsonElement("context")]
    public string Context { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}

public class QuestionSuggestionRequest
{
    public string SurveyTitle { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> ExistingQuestions { get; set; } = new List<string>();
    public int NumberOfSuggestions { get; set; } = 5;
}
