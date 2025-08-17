using MongoDB.Driver;
using SurveyApp.Models;
using Microsoft.Extensions.Options;

namespace SurveyApp.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IMongoCollection<UserProfile> _userProfiles;

    public UserProfileRepository(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userProfiles = mongoDatabase.GetCollection<UserProfile>("user_profiles");
    }

    public async Task<UserProfile?> GetByUserIdAsync(int userId)
    {
        return await _userProfiles.Find(up => up.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<UserProfile> CreateAsync(UserProfile userProfile)
    {
        // Auto-increment ID
        var maxId = await _userProfiles.Find(_ => true).SortByDescending(up => up.Id).Limit(1).FirstOrDefaultAsync();
        userProfile.Id = maxId?.Id + 1 ?? 1;
        
        await _userProfiles.InsertOneAsync(userProfile);
        return userProfile;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
    {
        userProfile.UpdatedAt = DateTime.UtcNow;
        await _userProfiles.ReplaceOneAsync(up => up.Id == userProfile.Id, userProfile);
        return userProfile;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _userProfiles.DeleteOneAsync(up => up.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _userProfiles.Find(up => up.Id == id).AnyAsync();
    }
}
