using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("questions")]
    public List<Question> Questions { get; set; } = new List<Question>();

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}

public class Question
{
    [BsonElement("question")]
    public string QuestionText { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("options")]
    public List<string>? Options { get; set; }

    [BsonElement("required")]
    public bool Required { get; set; } = false;
} 