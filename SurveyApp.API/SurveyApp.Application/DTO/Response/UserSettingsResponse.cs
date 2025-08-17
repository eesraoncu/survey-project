namespace SurveyApp.Application.DTO.Response;

public class UserSettingsResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Genel Ayarlar
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }

    // Bildirim Ayarları
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool WeeklyReports { get; set; }
    public bool SurveyReminders { get; set; }

    // Gizlilik Ayarları
    public bool DataAnalytics { get; set; }
    public bool ThirdPartyIntegrations { get; set; }
    public string ProfileVisibility { get; set; } = string.Empty;

    // Görünüm Ayarları
    public string Theme { get; set; } = string.Empty;
    public string ColorScheme { get; set; } = string.Empty;
    public string FontSize { get; set; } = string.Empty;

    // Dil Ayarları
    public string Language { get; set; } = string.Empty;
    public string DateFormat { get; set; } = string.Empty;
    public string TimeFormat { get; set; } = string.Empty;

    // Güvenlik Ayarları
    public bool TwoFactorEnabled { get; set; }
    public bool LoginNotifications { get; set; }
    public int SessionTimeout { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
