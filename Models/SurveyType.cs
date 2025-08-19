using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class QuestionType
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("question_type_name")]
    public string QuestionTypeName { get; set; } = string.Empty;

    [BsonElement("question_type_code")]
    public string QuestionTypeCode { get; set; } = string.Empty; // short_text, paragraph, multiple_choice, etc.

    [BsonElement("requires_choices")]
    public bool RequiresChoices { get; set; } = false; // çoktan seçmeli, çoklu seçim, açılır liste için true

    [BsonElement("allows_multiple_selection")]
    public bool AllowsMultipleSelection { get; set; } = false; // çoklu seçim için true

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

 