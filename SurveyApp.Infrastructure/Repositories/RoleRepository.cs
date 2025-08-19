using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IMongoCollection<Role> _roles;
    private readonly AutoIncrementService _autoIncrementService;

    public RoleRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _roles = database.GetCollection<Role>("role");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _roles.Find(_ => true).ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _roles.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        // Eğer ID zaten set edilmişse (manual), kullan; değilse otomatik ata
        if (role.Id == 0)
        {
            role.Id = await _autoIncrementService.GetNextIdAsync("role");
        }
        await _roles.InsertOneAsync(role);
        return role;
    }

    public async Task<bool> UpdateAsync(int id, Role role)
    {
        var result = await _roles.ReplaceOneAsync(r => r.Id == id, role);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _roles.DeleteOneAsync(r => r.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Role>> GetByIdsAsync(List<int> ids)
    {
        var filter = Builders<Role>.Filter.In(r => r.Id, ids);
        return await _roles.Find(filter).ToListAsync();
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        Console.WriteLine($"🔍 GetByNameAsync araniyor: '{roleName}'");
        
        // Önce IsActive kontrolü olmadan ara
        var roleWithoutActive = await _roles.Find(r => r.RoleName == roleName).FirstOrDefaultAsync();
        if (roleWithoutActive != null)
        {
            Console.WriteLine($"📌 Rol bulundu (IsActive={roleWithoutActive.IsActive}): {roleWithoutActive.RoleName}");
        }
        else
        {
            Console.WriteLine($"❌ Rol hiç bulunamadi: '{roleName}'");
        }
        
        // IsActive kontrolünü geçici olarak kaldır - sorun burada
        var result = await _roles.Find(r => r.RoleName == roleName).FirstOrDefaultAsync();
        Console.WriteLine($"🎯 Final sonuç (IsActive olmadan): {(result != null ? "BULUNDU" : "BULUNAMADI")}");
        return result;
    }
}
