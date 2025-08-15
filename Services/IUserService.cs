using SurveyApp.Models;

namespace SurveyApp.Services;

public interface IUserService
{
    Task<User?> GetUserWithRolesAsync(int userId);
    Task<User?> GetUserWithRolesByEmailAsync(string email);
    Task<List<User>> GetAllUsersWithRolesAsync();
    Task<bool> AddRoleToUserAsync(int userId, int roleId);
    Task<bool> AddRoleToUserAsync(int userId, string roleName);
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
    Task<bool> UserHasRoleAsync(int userId, string roleName);
    Task<bool> UserHasRoleAsync(int userId, int roleId);
}
