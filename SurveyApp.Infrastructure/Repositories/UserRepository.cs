using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<bool> UpdateAsync(string id, User user)
    {
        var result = await _users.ReplaceOneAsync(u => u.Id == id, user);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.UserEmail == email).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetByRoleIdAsync(string roleId)
    {
        return await _users.Find(u => u.RoleId == roleId).ToListAsync();
    }

    public async Task<List<User>> GetByAddressIdAsync(string addressId)
    {
        return await _users.Find(u => u.AddressId == addressId).ToListAsync();
    }
} 