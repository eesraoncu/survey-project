using SurveyApp.Models;

namespace SurveyApp.Services;

public interface IJiraAuthService
{
    string GetAuthorizationUrl();
    Task<User?> AuthenticateJiraUserAsync(string accessToken);
}
