using MongoDB.Driver;
using MongoDB.Bson;
using SurveyApp.Models;
using Microsoft.Extensions.Options;

namespace SurveyApp.Infrastructure.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly IMongoCollection<ActivityLog> _activityLogs;

    public ActivityLogRepository(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _activityLogs = mongoDatabase.GetCollection<ActivityLog>("activity_logs");
    }

    public async Task<List<ActivityLog>> GetAllAsync()
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        return await _activityLogs.Find(typeInt32Filter).SortByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<ActivityLog?> GetByIdAsync(int id)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var idFilter = Builders<ActivityLog>.Filter.Eq(x => x.Id, id);
        return await _activityLogs.Find(Builders<ActivityLog>.Filter.And(typeInt32Filter, idFilter)).FirstOrDefaultAsync();
    }

    public async Task<List<ActivityLog>> GetByUserIdAsync(int userId)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var userFilter = Builders<ActivityLog>.Filter.Eq(x => x.UserId, userId);
        return await _activityLogs.Find(Builders<ActivityLog>.Filter.And(typeInt32Filter, userFilter))
            .SortByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<List<ActivityLog>> GetByActivityTypeAsync(string activityType)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var activityFilter = Builders<ActivityLog>.Filter.Eq(x => x.ActivityType, activityType);
        return await _activityLogs.Find(Builders<ActivityLog>.Filter.And(typeInt32Filter, activityFilter))
            .SortByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<List<ActivityLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var dateFilter = Builders<ActivityLog>.Filter.And(
            Builders<ActivityLog>.Filter.Gte(x => x.CreatedAt, startDate),
            Builders<ActivityLog>.Filter.Lte(x => x.CreatedAt, endDate)
        );
        return await _activityLogs.Find(Builders<ActivityLog>.Filter.And(typeInt32Filter, dateFilter))
            .SortByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<List<ActivityLog>> GetByResourceTypeAsync(string resourceType, int? resourceId = null)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var resourceTypeFilter = Builders<ActivityLog>.Filter.Eq(x => x.ResourceType, resourceType);
        
        if (resourceId.HasValue)
        {
            var resourceIdFilter = Builders<ActivityLog>.Filter.Eq(x => x.ResourceId, resourceId.Value);
            return await _activityLogs.Find(Builders<ActivityLog>.Filter.And(typeInt32Filter, resourceTypeFilter, resourceIdFilter))
                .SortByDescending(x => x.CreatedAt).ToListAsync();
        }
        
        return await _activityLogs.Find(Builders<ActivityLog>.Filter.And(typeInt32Filter, resourceTypeFilter))
            .SortByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<ActivityLog> CreateAsync(ActivityLog activityLog)
    {
        // Auto-increment ID
        var maxId = await _activityLogs.Find(_ => true).SortByDescending(x => x.Id).Limit(1).FirstOrDefaultAsync();
        activityLog.Id = maxId?.Id + 1 ?? 1;
        
        await _activityLogs.InsertOneAsync(activityLog);
        return activityLog;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var idFilter = Builders<ActivityLog>.Filter.Eq(x => x.Id, id);
        var result = await _activityLogs.DeleteOneAsync(Builders<ActivityLog>.Filter.And(typeInt32Filter, idFilter));
        return result.DeletedCount > 0;
    }

    public async Task<long> GetCountAsync()
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        return await _activityLogs.CountDocumentsAsync(typeInt32Filter);
    }

    public async Task<List<ActivityLog>> GetRecentAsync(int limit = 50)
    {
        var typeInt32Filter = (FilterDefinition<ActivityLog>) new BsonDocument("_id", new BsonDocument("$type", 16));
        return await _activityLogs.Find(typeInt32Filter)
            .SortByDescending(x => x.CreatedAt)
            .Limit(limit)
            .ToListAsync();
    }
}
