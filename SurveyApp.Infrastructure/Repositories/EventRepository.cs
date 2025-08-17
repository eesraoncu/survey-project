using MongoDB.Driver;
using SurveyApp.Models;
using Microsoft.Extensions.Options;

namespace SurveyApp.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<Event> _events;

    public EventRepository(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _events = mongoDatabase.GetCollection<Event>("events");
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _events.Find(e => e.IsActive).ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _events.Find(e => e.Id == id && e.IsActive).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event>> GetByUserIdAsync(int userId)
    {
        return await _events.Find(e => e.UserId == userId && e.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _events.Find(e => e.EventDate >= startDate && e.EventDate <= endDate && e.IsActive).ToListAsync();
    }

    public async Task<Event> CreateAsync(Event eventItem)
    {
        // Auto-increment ID
        var maxId = await _events.Find(_ => true).SortByDescending(e => e.Id).Limit(1).FirstOrDefaultAsync();
        eventItem.Id = maxId?.Id + 1 ?? 1;
        
        await _events.InsertOneAsync(eventItem);
        return eventItem;
    }

    public async Task<Event> UpdateAsync(Event eventItem)
    {
        eventItem.UpdatedAt = DateTime.UtcNow;
        await _events.ReplaceOneAsync(e => e.Id == eventItem.Id, eventItem);
        return eventItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var update = Builders<Event>.Update.Set(e => e.IsActive, false).Set(e => e.UpdatedAt, DateTime.UtcNow);
        var result = await _events.UpdateOneAsync(e => e.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _events.Find(e => e.Id == id && e.IsActive).AnyAsync();
    }
}
