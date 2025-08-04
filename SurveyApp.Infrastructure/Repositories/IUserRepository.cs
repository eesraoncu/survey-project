using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User> CreateAsync(User user);
    Task<bool> UpdateAsync(string id, User user);
    Task<bool> DeleteAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetByRoleIdAsync(string roleId);
    Task<List<User>> GetByAddressIdAsync(string addressId);
} 