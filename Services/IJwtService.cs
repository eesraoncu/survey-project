using SurveyApp.Models;

namespace SurveyApp.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}
