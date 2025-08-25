using MongoDB.Driver;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class AdminRoleSyncService
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUserRoleRepository _userRoles;
    private readonly IMongoDatabase _db;

    public AdminRoleSyncService(IUserRepository users, IRoleRepository roles, IUserRoleRepository userRoles, IMongoDatabase db)
    {
        _users = users;
        _roles = roles;
        _userRoles = userRoles;
        _db = db;
    }

    public async Task SyncAsync()
    {
        var adminCollection = _db.GetCollection<Admin>("admin");
        var admins = await adminCollection.Find(_ => true).ToListAsync();
        if (admins.Count == 0) return;

        // 1) admin rolünü garanti et
        var adminRole = await _roles.GetByNameAsync("admin");
        if (adminRole is null)
        {
            adminRole = await _roles.CreateAsync(new Role
            {
                RoleName = "admin",
                IsActive = true
            });
        }

        // 2) her admin email için user'ı upsert et ve admin rolünü bağla
        var allUsers = await _users.GetAllAsync();
        foreach (var admin in admins)
        {
            var user = allUsers.FirstOrDefault(u => u.UserEmail == admin.AdminEmail);
            if (user is null)
            {
                // Users'ta yoksa oluştur
                user = new User
                {
                    UserEmail = admin.AdminEmail,
                    UserName = admin.AdminName,
                    UserSurname = admin.AdminSurname,
                    IsActive = true,
                    // admin_password veritabanında SHA-256 Base64 ise doğrudan saklayabiliriz
                    // PasswordService.VerifyPassword bu formatı destekliyor
                    UserPassword = admin.AdminPassword,
                    CreatedAt = DateTime.UtcNow,
                    Roles = new List<Role>()
                };
                await _users.CreateAsync(user);
            }
            else
            {
                // Parolayı senkron tutmak isterseniz (opsiyonel):
                // user.UserPassword = admin.AdminPassword;
                // await _users.UpdateAsync(user.Id, user);
            }

            // admin rolü ilişkisini ekle
            var has = await _userRoles.ExistsAsync(user.Id, adminRole.Id);
            if (!has)
            {
                await _userRoles.AddRoleToUserAsync(user.Id, adminRole.Id);
            }
        }
    }
}
