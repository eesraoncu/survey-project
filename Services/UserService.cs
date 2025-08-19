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
        Console.WriteLine($"🔍 Rol araniyor: {roleName}");
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) 
        {
            Console.WriteLine($"❌ Rol bulunamadi: {roleName}");
            // Tüm rolleri listele
            var allRoles = await _roleRepository.GetAllAsync();
            Console.WriteLine($"📋 Mevcut roller:");
            foreach(var r in allRoles)
            {
                Console.WriteLine($"   ID={r.Id}, Name={r.RoleName}, Active={r.IsActive}");
            }
            return false;
        }
        Console.WriteLine($"✅ Rol bulundu: {roleName} (ID={role.Id})");

        var result = await _userRoleRepository.AddRoleToUserAsync(userId, role.Id);
        Console.WriteLine($"📝 user_roles tablosuna ekleme: userId={userId}, roleId={role.Id}, sonuc={result}");
        return result;
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

    // Anket oluşturduğunda owner rolü ekleme
    public async Task<bool> MakeUserOwnerForSurveyAsync(int userId, int surveyId)
    {
        // Admin kullanıcıları için rol değişikliği yapma
        if (await UserHasRoleAsync(userId, "admin"))
        {
            return true; // Admin zaten her şeyi yapabilir
        }

        // User'a owner rolü ekle (user rolü kalır)
        return await AddRoleToUserAsync(userId, "owner");
    }

    // Anket cevaplarken user rolüne geçiş
    public async Task<bool> EnsureUserRoleForAnsweringAsync(int userId, int surveyId)
    {
        // Admin kullanıcıları için rol değişikliği yapma
        if (await UserHasRoleAsync(userId, "admin"))
        {
            return true; // Admin zaten her şeyi yapabilir
        }

        // Kullanıcının bu anketi oluşturup oluşturmadığını kontrol etmek için
        // Survey service'den yardım alacağız, şimdilik sadece user rolü olduğundan emin ol
        var hasUserRole = await UserHasRoleAsync(userId, "user");
        if (!hasUserRole)
        {
            return await AddRoleToUserAsync(userId, "user");
        }
        
        return true;
    }

    // Kullanıcının belirli bir anket için owner olup olmadığını kontrol et
    public async Task<bool> IsUserOwnerOfSurveyAsync(int userId, int surveyCreatorId)
    {
        // Admin her zaman owner gibi davranabilir
        if (await UserHasRoleAsync(userId, "admin"))
        {
            return true;
        }

        // Kullanıcı anketin yaratıcısı mı?
        return userId == surveyCreatorId;
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
