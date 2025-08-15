using SurveyApp.Models;

namespace SurveyApp.Services;

public interface ITrelloAuthService
{
    string GetAuthorizationUrl();
    Task<User?> AuthenticateTrelloUserAsync(string code);
    Task<User?> GetOrCreateUserFromTrelloAsync(string email, string fullName, string username);
}
