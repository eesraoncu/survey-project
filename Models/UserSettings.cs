using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class UserSettings
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UserId { get; set; }

    // Genel Ayarlar
    [BsonElement("profile_picture")]
    public string? ProfilePicture { get; set; }

    [BsonElement("bio")]
    public string? Bio { get; set; }

    [BsonElement("phone_number")]
    public string? PhoneNumber { get; set; }

    [BsonElement("website")]
    public string? Website { get; set; }

    // Bildirim Ayarları
    [BsonElement("email_notifications")]
    public bool EmailNotifications { get; set; } = true;

    [BsonElement("push_notifications")]
    public bool PushNotifications { get; set; } = true;

    [BsonElement("weekly_reports")]
    public bool WeeklyReports { get; set; } = false;

    [BsonElement("survey_reminders")]
    public bool SurveyReminders { get; set; } = true;

    // Gizlilik Ayarları
    [BsonElement("data_analytics")]
    public bool DataAnalytics { get; set; } = true;

    [BsonElement("third_party_integrations")]
    public bool ThirdPartyIntegrations { get; set; } = false;

    [BsonElement("profile_visibility")]
    public string ProfileVisibility { get; set; } = "public"; // public, private, friends

    // Görünüm Ayarları
    [BsonElement("theme")]
    public string Theme { get; set; } = "light"; // light, dark, auto

    [BsonElement("color_scheme")]
    public string ColorScheme { get; set; } = "default"; // default, blue, green, purple

    [BsonElement("font_size")]
    public string FontSize { get; set; } = "medium"; // small, medium, large

    // Dil Ayarları
    [BsonElement("language")]
    public string Language { get; set; } = "tr"; // tr, en, de, fr

    [BsonElement("date_format")]
    public string DateFormat { get; set; } = "dd/MM/yyyy";

    [BsonElement("time_format")]
    public string TimeFormat { get; set; } = "24"; // 12, 24

    // Güvenlik Ayarları
    [BsonElement("two_factor_enabled")]
    public bool TwoFactorEnabled { get; set; } = false;

    [BsonElement("login_notifications")]
    public bool LoginNotifications { get; set; } = true;

    [BsonElement("session_timeout")]
    public int SessionTimeout { get; set; } = 30; // dakika

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
