using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IUserRoleRepository
{
    Task<List<UserRole>> GetAllAsync();
    Task<List<UserRole>> GetByUserIdAsync(int userId);
    Task<UserRole?> GetByIdAsync(int id);
    Task<UserRole> CreateAsync(UserRole userRole);
    Task<bool> UpdateAsync(UserRole userRole);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int userId, int roleId);
    Task<bool> AddRoleToUserAsync(int userId, int roleId);
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
}
