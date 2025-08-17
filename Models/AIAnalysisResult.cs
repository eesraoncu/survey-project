using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class AIAnalysisResult
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("survey_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveyId { get; set; }

    [BsonElement("analysis_type")]
    public string AnalysisType { get; set; } = string.Empty; // "sentiment", "summary", "insights", "recommendations"

    [BsonElement("analysis_result")]
    public string AnalysisResult { get; set; } = string.Empty;

    [BsonElement("confidence_score")]
    public double ConfidenceScore { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("created_by")]
    [BsonRepresentation(BsonType.Int32)]
    public int CreatedBy { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}

public class SurveyInsight
{
    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("insight")]
    public string Insight { get; set; } = string.Empty;

    [BsonElement("importance")]
    public string Importance { get; set; } = string.Empty; // "high", "medium", "low"
}

public class SentimentAnalysis
{
    [BsonElement("overall_sentiment")]
    public string OverallSentiment { get; set; } = string.Empty; // "positive", "negative", "neutral"

    [BsonElement("positive_percentage")]
    public double PositivePercentage { get; set; }

    [BsonElement("negative_percentage")]
    public double NegativePercentage { get; set; }

    [BsonElement("neutral_percentage")]
    public double NeutralPercentage { get; set; }

    [BsonElement("key_positive_themes")]
    public List<string> KeyPositiveThemes { get; set; } = new List<string>();

    [BsonElement("key_negative_themes")]
    public List<string> KeyNegativeThemes { get; set; } = new List<string>();
}
