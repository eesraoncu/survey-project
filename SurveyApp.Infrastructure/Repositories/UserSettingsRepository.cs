using MongoDB.Driver;
using SurveyApp.Models;
using Microsoft.Extensions.Options;

namespace SurveyApp.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly IMongoCollection<UserSettings> _userSettings;

    public UserSettingsRepository(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userSettings = mongoDatabase.GetCollection<UserSettings>("user_settings");
    }

    public async Task<UserSettings?> GetByUserIdAsync(int userId)
    {
        return await _userSettings.Find(us => us.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<UserSettings> CreateAsync(UserSettings userSettings)
    {
        // Auto-increment ID
        var maxId = await _userSettings.Find(_ => true).SortByDescending(us => us.Id).Limit(1).FirstOrDefaultAsync();
        userSettings.Id = maxId?.Id + 1 ?? 1;
        
        await _userSettings.InsertOneAsync(userSettings);
        return userSettings;
    }

    public async Task<UserSettings> UpdateAsync(UserSettings userSettings)
    {
        userSettings.UpdatedAt = DateTime.UtcNow;
        await _userSettings.ReplaceOneAsync(us => us.Id == userSettings.Id, userSettings);
        return userSettings;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _userSettings.DeleteOneAsync(us => us.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _userSettings.Find(us => us.Id == id).AnyAsync();
    }
}
