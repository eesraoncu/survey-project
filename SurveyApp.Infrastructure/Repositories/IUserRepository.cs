using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
    Task<bool> UpdateAsync(int id, User user);
    Task<bool> DeleteAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetByRoleIdAsync(int roleId);
    Task<List<User>> GetByAddressIdAsync(int addressId);
} 