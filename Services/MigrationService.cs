using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class MigrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;

    public MigrationService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    public async Task MigrateUsersToNewRoleSystem()
    {
        try
        {
            // 1. Rolleri oluştur (eğer yoksa)
            await EnsureRolesExist();

            // 2. Mevcut kullanıcıları al
            var users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                // Eski RoleId'ye göre rol ekle
                await MigrateUserRole(user);
            }

            Console.WriteLine($"Migration tamamlandı. {users.Count} kullanıcı güncellendi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration hatası: {ex.Message}");
            throw;
        }
    }

    private async Task EnsureRolesExist()
    {
        var roles = await _roleRepository.GetAllAsync();
        var roleNames = roles.Select(r => r.RoleName).ToList();

        if (!roleNames.Contains("admin"))
        {
            await _roleRepository.CreateAsync(new Role { RoleName = "admin" });
        }

        if (!roleNames.Contains("owner"))
        {
            await _roleRepository.CreateAsync(new Role { RoleName = "owner" });
        }

        if (!roleNames.Contains("user"))
        {
            await _roleRepository.CreateAsync(new Role { RoleName = "user" });
        }
    }

    private async Task MigrateUserRole(User user)
    {
        // Eski role_id alanından rol bilgisini al
        var userRole = await _roleRepository.GetByNameAsync("user");
        if (userRole != null)
        {
            // Eğer kullanıcının eski role_id'si varsa, ona göre rol ekle
            if (user.OldRoleId.HasValue)
            {
                var role = await _roleRepository.GetByIdAsync(user.OldRoleId.Value);
                if (role != null)
                {
                    await _userRoleRepository.AddRoleToUserAsync(user.Id, role.Id);
                }
                else
                {
                    // Eski role_id geçersizse User rolü ekle
                    await _userRoleRepository.AddRoleToUserAsync(user.Id, userRole.Id);
                }
            }
            else
            {
                // Eski role_id yoksa User rolü ekle
                await _userRoleRepository.AddRoleToUserAsync(user.Id, userRole.Id);
            }
        }
        
        // Eğer kullanıcının hiç rolü yoksa, User rolü ekle
        var existingRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
        if (!existingRoles.Any())
        {
            await _userRoleRepository.AddRoleToUserAsync(user.Id, userRole.Id);
        }
    }
}
