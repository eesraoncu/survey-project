namespace SurveyApp.Application.DTO.Request;

public class UserSettingsRequest
{
    // Genel Ayarlar
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }

    // Bildirim Ayarları
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool WeeklyReports { get; set; } = false;
    public bool SurveyReminders { get; set; } = true;

    // Gizlilik Ayarları
    public bool DataAnalytics { get; set; } = true;
    public bool ThirdPartyIntegrations { get; set; } = false;
    public string ProfileVisibility { get; set; } = "public";

    // Görünüm Ayarları
    public string Theme { get; set; } = "light";
    public string ColorScheme { get; set; } = "default";
    public string FontSize { get; set; } = "medium";

    // Dil Ayarları
    public string Language { get; set; } = "tr";
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "24";

    // Güvenlik Ayarları
    public bool TwoFactorEnabled { get; set; } = false;
    public bool LoginNotifications { get; set; } = true;
    public int SessionTimeout { get; set; } = 30;
}

public class UserSettingsUpdateRequest : UserSettingsRequest
{
    public int Id { get; set; }
}
