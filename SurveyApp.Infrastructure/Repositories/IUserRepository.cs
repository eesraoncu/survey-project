using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetByRoleIdAsync(string roleId);
    Task<List<User>> GetByAddressIdAsync(string addressId);
    Task CreateAsync(User user);
    Task UpdateAsync(string id, User user);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<bool> EmailExistsAsync(string email);
} 