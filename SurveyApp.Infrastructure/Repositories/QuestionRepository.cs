using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _collection;

    public QuestionRepository(IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
        
        if (mongoSettings == null)
            throw new ArgumentNullException(nameof(mongoSettings), "MongoDB ayarları bulunamadı!");

        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        var client = new MongoClient(settings);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<Question>("questions");
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(string id)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Question>> GetBySurveyIdAsync(string surveyId)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.SurveysId, surveyId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<Question>> GetBySurveyIdOrderedAsync(string surveyId)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.SurveysId, surveyId);
        var sort = Builders<Question>.Sort.Ascending(x => x.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task CreateAsync(Question question)
    {
        await _collection.InsertOneAsync(question);
    }

    public async Task UpdateAsync(string id, Question question)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
        await _collection.ReplaceOneAsync(filter, question);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task DeleteBySurveyIdAsync(string surveyId)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.SurveysId, surveyId);
        await _collection.DeleteManyAsync(filter);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).AnyAsync();
    }
} 