using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class ActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UserId { get; set; }

    [BsonElement("activity_type")]
    public string ActivityType { get; set; } = string.Empty; // login, logout, survey_created, survey_updated, etc.

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty; // Detaylı açıklama

    [BsonElement("ip_address")]
    public string? IpAddress { get; set; }

    [BsonElement("user_agent")]
    public string? UserAgent { get; set; }

    [BsonElement("resource_id")]
    public int? ResourceId { get; set; } // İlgili kaynağın ID'si (anket ID, soru ID, etc.)

    [BsonElement("resource_type")]
    public string? ResourceType { get; set; } // survey, question, answer, etc.

    [BsonElement("additional_data")]
    public Dictionary<string, object>? AdditionalData { get; set; } // Ek bilgiler

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_successful")]
    public bool IsSuccessful { get; set; } = true; // İşlem başarılı mı?

    [BsonElement("error_message")]
    public string? ErrorMessage { get; set; } // Hata varsa mesajı
}
