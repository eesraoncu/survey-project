using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class SurveyCreateRequest
{
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public int UsersId { get; set; }
    public int SurveyTypeId { get; set; }
    
    // Anket özellikleri
    public bool AllowsAnonymousResponses { get; set; } = true;
    public bool RequiresLogin { get; set; } = false;
    public int? MaxResponses { get; set; } = null;
    public DateTime? ExpiresAt { get; set; } = null;
    public bool AllowsEditing { get; set; } = false;
    public bool ShowsResults { get; set; } = false;
    
    // Özellik aktivasyonları (görseldeki 6 özellik)
    public bool EnablesRating { get; set; } = false;
    public int RatingScaleMax { get; set; } = 5;
    public int RatingScaleMin { get; set; } = 1;
    
    public bool EnablesDateFields { get; set; } = false;
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    
    public bool EnablesLocation { get; set; } = false;
    public string LocationPrecision { get; set; } = "city";
    
    public bool EnablesPhone { get; set; } = false;
    public string PhoneFormat { get; set; } = "TR";
    
    public bool EnablesEmail { get; set; } = false;
    public bool EmailVerification { get; set; } = false;
    
    public bool EnablesFullName { get; set; } = false;
    public string NameFormat { get; set; } = "first_last";
    
    // Arka plan resmi
    public string? SurveyBackgroundImage { get; set; } = null;
}

public sealed class SurveyUpdateRequest
{
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public int UsersId { get; set; } // Log için gerekli
    public int SurveyTypeId { get; set; }
    public bool IsActive { get; set; }
    
    // Anket özellikleri
    public bool AllowsAnonymousResponses { get; set; } = true;
    public bool RequiresLogin { get; set; } = false;
    public int? MaxResponses { get; set; } = null;
    public DateTime? ExpiresAt { get; set; } = null;
    public bool AllowsEditing { get; set; } = false;
    public bool ShowsResults { get; set; } = false;
    
    // Özellik aktivasyonları (görseldeki 6 özellik)
    public bool EnablesRating { get; set; } = false;
    public int RatingScaleMax { get; set; } = 5;
    public int RatingScaleMin { get; set; } = 1;
    
    public bool EnablesDateFields { get; set; } = false;
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    
    public bool EnablesLocation { get; set; } = false;
    public string LocationPrecision { get; set; } = "city";
    
    public bool EnablesPhone { get; set; } = false;
    public string PhoneFormat { get; set; } = "TR";
    
    public bool EnablesEmail { get; set; } = false;
    public bool EmailVerification { get; set; } = false;
    
    public bool EnablesFullName { get; set; } = false;
    public string NameFormat { get; set; } = "first_last";
    
    // Arka plan resmi
    public string? SurveyBackgroundImage { get; set; } = null;
}

public sealed class SurveyCompletionRequest
{
    public int UserId { get; set; }
}