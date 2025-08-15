using SurveyApp.Models;

namespace SurveyApp.Services;

public interface IGoogleAuthService
{
    Task<User?> AuthenticateGoogleUserAsync(string accessToken);
    Task<User?> GetOrCreateUserFromGoogleAsync(string email, string name, string surname);
}
