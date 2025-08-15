using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<List<Role>> GetByIdsAsync(List<int> ids);
    Task<Role?> GetByNameAsync(string roleName);
    Task<Role> CreateAsync(Role role);
    Task<bool> UpdateAsync(int id, Role role);
    Task<bool> DeleteAsync(int id);
}
