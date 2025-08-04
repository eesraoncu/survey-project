using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface ISurveyRepository
{
    Task<List<Survey>> GetAllAsync();
    Task<Survey?> GetByIdAsync(string id);
    Task<Survey> CreateAsync(Survey survey);
    Task<bool> UpdateAsync(string id, Survey survey);
    Task<bool> DeleteAsync(string id);
    Task<List<Survey>> GetByUserIdAsync(string userId);
    Task<List<Survey>> GetBySurveyTypeIdAsync(string surveyTypeId);
    Task<List<Survey>> GetActiveSurveysAsync();
} 