namespace SurveyApp.Models;

public class JiraAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string AuthorizationUrl { get; set; } = "https://auth.atlassian.com/authorize";
    public string TokenUrl { get; set; } = "https://auth.atlassian.com/oauth/token";
    public string UserInfoUrl { get; set; } = "https://api.atlassian.com/me";
}
