using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
        
        if (mongoSettings == null)
            throw new ArgumentNullException(nameof(mongoSettings), "MongoDB ayarları bulunamadı!");

        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        var client = new MongoClient(settings);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<User>("users");
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserEmail, email);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetByRoleIdAsync(string roleId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.RoleId, roleId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<User>> GetByAddressIdAsync(string addressId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.AddressId, addressId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _collection.InsertOneAsync(user);
    }

    public async Task UpdateAsync(string id, User user)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, id);
        await _collection.ReplaceOneAsync(filter, user);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserEmail, email);
        return await _collection.Find(filter).AnyAsync();
    }
} 