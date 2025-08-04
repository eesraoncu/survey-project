using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class SurveyRepository : ISurveyRepository
{
    private readonly IMongoCollection<Survey> _surveys;

    public SurveyRepository(IMongoDatabase database)
    {
        _surveys = database.GetCollection<Survey>("surveys");
    }

    public async Task<List<Survey>> GetAllAsync()
    {
        return await _surveys.Find(_ => true).ToListAsync();
    }

    public async Task<Survey?> GetByIdAsync(string id)
    {
        return await _surveys.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Survey> CreateAsync(Survey survey)
    {
        await _surveys.InsertOneAsync(survey);
        return survey;
    }

    public async Task<bool> UpdateAsync(string id, Survey survey)
    {
        var result = await _surveys.ReplaceOneAsync(s => s.Id == id, survey);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _surveys.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Survey>> GetByUserIdAsync(string userId)
    {
        return await _surveys.Find(s => s.UsersId == userId).ToListAsync();
    }

    public async Task<List<Survey>> GetBySurveyTypeIdAsync(string surveyTypeId)
    {
        return await _surveys.Find(s => s.SurveyTypeId == surveyTypeId).ToListAsync();
    }

    public async Task<List<Survey>> GetActiveSurveysAsync()
    {
        return await _surveys.Find(s => s.IsActive).ToListAsync();
    }
} 