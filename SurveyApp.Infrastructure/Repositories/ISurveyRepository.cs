using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface ISurveyRepository
{
    Task<List<Survey>> GetAllAsync();
    Task<Survey?> GetByIdAsync(string id);
    Task<List<Survey>> GetByUserIdAsync(string userId);
    Task<List<Survey>> GetBySurveyTypeIdAsync(string surveyTypeId);
    Task<List<Survey>> GetActiveSurveysAsync();
    Task CreateAsync(Survey survey);
    Task UpdateAsync(string id, Survey survey);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
} 