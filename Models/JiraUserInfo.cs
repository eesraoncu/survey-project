using System.Text.Json.Serialization;

namespace SurveyApp.Models;

public class JiraUserInfo
{
    [JsonPropertyName("account_id")]
    public string AccountId { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;
    
    [JsonPropertyName("account_type")]
    public string AccountType { get; set; } = string.Empty;
    
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;
    
    [JsonPropertyName("zoneinfo")]
    public string ZoneInfo { get; set; } = string.Empty;
    
    [JsonPropertyName("locale")]
    public string Locale { get; set; } = string.Empty;
    
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
    
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty;
    
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;
}
