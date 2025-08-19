using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly IMongoDatabase _database;
    private readonly AutoIncrementService _autoIncrementService;

    public UserRoleRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _database = database;
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<UserRole>> GetAllAsync()
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<List<UserRole>> GetByUserIdAsync(int userId)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        return await collection.Find(x => x.UserId == userId && x.IsActive).ToListAsync();
    }

    public async Task<UserRole?> GetByIdAsync(int id)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<UserRole> CreateAsync(UserRole userRole)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        userRole.Id = await _autoIncrementService.GetNextIdAsync("user_roles");
        await collection.InsertOneAsync(userRole);
        return userRole;
    }

    public async Task<bool> UpdateAsync(UserRole userRole)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        var result = await collection.ReplaceOneAsync(x => x.Id == userRole.Id, userRole);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        var result = await collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(int userId, int roleId)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        return await collection.Find(x => x.UserId == userId && x.RoleId == roleId && x.IsActive).AnyAsync();
    }

    public async Task<bool> AddRoleToUserAsync(int userId, int roleId)
    {
        Console.WriteLine($"🔄 AddRoleToUserAsync: userId={userId}, roleId={roleId}");
        
        // Eğer rol zaten varsa ekleme
        var exists = await ExistsAsync(userId, roleId);
        Console.WriteLine($"🔍 Rol zaten var mı? {exists}");
        if (exists)
            return true;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        Console.WriteLine($"📋 UserRole oluşturuldu: {userRole.UserId}-{userRole.RoleId}");

        try
        {
            var created = await CreateAsync(userRole);
            Console.WriteLine($"✅ user_roles'a eklendi: ID={created.Id}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ user_roles ekleme HATASI: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        var collection = _database.GetCollection<UserRole>("user_roles");
        var update = Builders<UserRole>.Update.Set(x => x.IsActive, false);
        var result = await collection.UpdateOneAsync(x => x.UserId == userId && x.RoleId == roleId, update);
        return result.ModifiedCount > 0;
    }
}
