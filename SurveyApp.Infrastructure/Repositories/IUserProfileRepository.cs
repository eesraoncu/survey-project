using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(int userId);
    Task<UserProfile> CreateAsync(UserProfile userProfile);
    Task<UserProfile> UpdateAsync(UserProfile userProfile);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
