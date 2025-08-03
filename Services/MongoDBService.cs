using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class MongoDBService
{
    private readonly IMongoCollection<Survey> _collection;

    public MongoDBService(IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
        
        if (mongoSettings == null)
            throw new ArgumentNullException(nameof(mongoSettings), "MongoDB ayarları bulunamadı!");

        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<Survey>(mongoSettings.CollectionName);
    }

    public async Task<List<Survey>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Survey?> GetByIdAsync(string id)
    {
        var filter = Builders<Survey>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Survey survey)
    {
        await _collection.InsertOneAsync(survey);
    }

    public async Task UpdateAsync(string id, Survey survey)
    {
        var filter = Builders<Survey>.Filter.Eq(x => x.Id, id);
        await _collection.ReplaceOneAsync(filter, survey);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Survey>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<List<Survey>> GetActiveSurveysAsync()
    {
        var filter = Builders<Survey>.Filter.Eq(x => x.IsActive, true);
        return await _collection.Find(filter).ToListAsync();
    }
} 