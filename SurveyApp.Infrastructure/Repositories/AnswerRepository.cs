using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly IMongoCollection<Answer> _collection;

    public AnswerRepository(IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
        
        if (mongoSettings == null)
            throw new ArgumentNullException(nameof(mongoSettings), "MongoDB ayarları bulunamadı!");

        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        var client = new MongoClient(settings);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<Answer>("answers");
    }

    public async Task<List<Answer>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Answer?> GetByIdAsync(string id)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Answer>> GetByQuestionIdAsync(string questionId)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.QuestionsId, questionId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<Answer>> GetBySurveyIdAsync(string surveyId)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.SurveysId, surveyId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<Answer>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.UsersId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Answer?> GetByQuestionAndUserAsync(string questionId, string userId)
    {
        var filter = Builders<Answer>.Filter.And(
            Builders<Answer>.Filter.Eq(x => x.QuestionsId, questionId),
            Builders<Answer>.Filter.Eq(x => x.UsersId, userId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Answer answer)
    {
        await _collection.InsertOneAsync(answer);
    }

    public async Task UpdateAsync(string id, Answer answer)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.Id, id);
        await _collection.ReplaceOneAsync(filter, answer);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task DeleteBySurveyIdAsync(string surveyId)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.SurveysId, surveyId);
        await _collection.DeleteManyAsync(filter);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var filter = Builders<Answer>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).AnyAsync();
    }
} 