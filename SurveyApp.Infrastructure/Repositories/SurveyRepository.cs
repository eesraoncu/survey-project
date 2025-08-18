using MongoDB.Driver;
using MongoDB.Bson;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class SurveyRepository : ISurveyRepository
{
    private readonly IMongoCollection<Survey> _surveys;
    private readonly AutoIncrementService _autoIncrementService;

    public SurveyRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _surveys = database.GetCollection<Survey>("surveys");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<Survey>> GetAllAsync()
    {
        // Sadece _id türü Int32 olan kayıtları getir (ObjectId olan eski kayıtlar deserialize hatası üretir)
        var typeInt32Filter = (FilterDefinition<Survey>) new BsonDocument("_id", new BsonDocument("$type", 16));
        return await _surveys.Find(typeInt32Filter).ToListAsync();
    }

    public async Task<Survey?> GetByIdAsync(int id)
    {
        return await _surveys.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Survey> CreateAsync(Survey survey)
    {
        // Otomatik ID ataması (sadece set edilmemişse)
        if (survey.Id == 0)
        {
            survey.Id = await _autoIncrementService.GetNextIdAsync("surveys");
        }
        await _surveys.InsertOneAsync(survey);
        return survey;
    }

    public async Task<bool> UpdateAsync(int id, Survey survey)
    {
        var result = await _surveys.ReplaceOneAsync(s => s.Id == id, survey);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _surveys.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Survey>> GetByUserIdAsync(int userId)
    {
        var typeInt32Filter = (FilterDefinition<Survey>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var userFilter = Builders<Survey>.Filter.Eq(s => s.UsersId, userId);
        return await _surveys.Find(Builders<Survey>.Filter.And(typeInt32Filter, userFilter)).ToListAsync();
    }

    public async Task<List<Survey>> GetBySurveyTypeIdAsync(int surveyTypeId)
    {
        var typeInt32Filter = (FilterDefinition<Survey>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var typeFilter = Builders<Survey>.Filter.Eq(s => s.SurveyTypeId, surveyTypeId);
        return await _surveys.Find(Builders<Survey>.Filter.And(typeInt32Filter, typeFilter)).ToListAsync();
    }

    public async Task<List<Survey>> GetActiveSurveysAsync()
    {
        var typeInt32Filter = (FilterDefinition<Survey>) new BsonDocument("_id", new BsonDocument("$type", 16));
        var activeFilter = Builders<Survey>.Filter.Eq(s => s.IsActive, true);
        return await _surveys.Find(Builders<Survey>.Filter.And(typeInt32Filter, activeFilter)).ToListAsync();
    }
} 