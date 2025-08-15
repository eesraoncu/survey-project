using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    public async Task<User?> GetUserWithRolesAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        await LoadUserRolesAsync(user);
        return user;
    }

    public async Task<User?> GetUserWithRolesByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        await LoadUserRolesAsync(user);
        return user;
    }

    public async Task<List<User>> GetAllUsersWithRolesAsync()
    {
        var users = await _userRepository.GetAllAsync();
        
        foreach (var user in users)
        {
            await LoadUserRolesAsync(user);
        }

        return users;
    }

    public async Task<bool> AddRoleToUserAsync(int userId, int roleId)
    {
        return await _userRoleRepository.AddRoleToUserAsync(userId, roleId);
    }

    public async Task<bool> AddRoleToUserAsync(int userId, string roleName)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return false;

        return await _userRoleRepository.AddRoleToUserAsync(userId, role.Id);
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        return await _userRoleRepository.RemoveRoleFromUserAsync(userId, roleId);
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, string roleName)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return false;

        return await _userRoleRepository.RemoveRoleFromUserAsync(userId, role.Id);
    }

    public async Task<bool> UserHasRoleAsync(int userId, string roleName)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return false;

        return await _userRoleRepository.ExistsAsync(userId, role.Id);
    }

    public async Task<bool> UserHasRoleAsync(int userId, int roleId)
    {
        return await _userRoleRepository.ExistsAsync(userId, roleId);
    }

    private async Task LoadUserRolesAsync(User user)
    {
        var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        
        if (roleIds.Any())
        {
            var roles = await _roleRepository.GetByIdsAsync(roleIds);
            user.Roles = roles.Where(r => r.IsActive).ToList();
        }
        else
        {
            user.Roles = new List<Role>();
        }
    }
}
