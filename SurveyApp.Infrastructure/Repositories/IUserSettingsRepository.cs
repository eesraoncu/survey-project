using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetByUserIdAsync(int userId);
    Task<UserSettings> CreateAsync(UserSettings userSettings);
    Task<UserSettings> UpdateAsync(UserSettings userSettings);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
