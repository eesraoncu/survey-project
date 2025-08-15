using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly AutoIncrementService _autoIncrementService;

    public UserRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _users = database.GetCollection<User>("users");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        // Otomatik ID ataması
        user.Id = await _autoIncrementService.GetNextIdAsync("users");
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<bool> UpdateAsync(int id, User user)
    {
        var result = await _users.ReplaceOneAsync(u => u.Id == id, user);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.UserEmail == email).FirstOrDefaultAsync();
    }

    public Task<List<User>> GetByRoleIdAsync(int roleId)
    {
        // Bu metod artık UserRole tablosundan çekilecek
        // Şimdilik boş liste döndür
        return Task.FromResult(new List<User>());
    }

    public async Task<List<User>> GetByAddressIdAsync(int addressId)
    {
        return await _users.Find(u => u.AddressId == addressId).ToListAsync();
    }
} 