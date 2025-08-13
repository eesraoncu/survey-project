using MongoDB.Driver;
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
        return await _surveys.Find(_ => true).ToListAsync();
    }

    public async Task<Survey?> GetByIdAsync(int id)
    {
        return await _surveys.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Survey> CreateAsync(Survey survey)
    {
        // Otomatik ID ataması
        survey.Id = await _autoIncrementService.GetNextIdAsync("surveys");
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
        return await _surveys.Find(s => s.UsersId == userId).ToListAsync();
    }

    public async Task<List<Survey>> GetBySurveyTypeIdAsync(int surveyTypeId)
    {
        return await _surveys.Find(s => s.SurveyTypeId == surveyTypeId).ToListAsync();
    }

    public async Task<List<Survey>> GetActiveSurveysAsync()
    {
        return await _surveys.Find(s => s.IsActive).ToListAsync();
    }
} 