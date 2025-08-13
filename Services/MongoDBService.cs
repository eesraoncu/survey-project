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

        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        
        // Timeout ayarları
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(60);
        settings.ConnectTimeout = TimeSpan.FromSeconds(60);
        settings.SocketTimeout = TimeSpan.FromSeconds(60);
        settings.HeartbeatTimeout = TimeSpan.FromSeconds(60);
        
        // SSL'i tamamen devre dışı bırak
        settings.SslSettings = new SslSettings
        {
            EnabledSslProtocols = System.Security.Authentication.SslProtocols.None,
            CheckCertificateRevocation = false
        };

        var client = new MongoClient(settings);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<Survey>(mongoSettings.CollectionName);
    }

    public async Task<List<Survey>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Survey?> GetByIdAsync(int id)
    {
        var filter = Builders<Survey>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Survey survey)
    {
        await _collection.InsertOneAsync(survey);
    }

    public async Task UpdateAsync(int id, Survey survey)
    {
        var filter = Builders<Survey>.Filter.Eq(x => x.Id, id);
        await _collection.ReplaceOneAsync(filter, survey);
    }

    public async Task DeleteAsync(int id)
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