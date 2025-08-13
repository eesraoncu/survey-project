using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface ISurveyRepository
{
    Task<List<Survey>> GetAllAsync();
    Task<Survey?> GetByIdAsync(int id);
    Task<Survey> CreateAsync(Survey survey);
    Task<bool> UpdateAsync(int id, Survey survey);
    Task<bool> DeleteAsync(int id);
    Task<List<Survey>> GetByUserIdAsync(int userId);
    Task<List<Survey>> GetBySurveyTypeIdAsync(int surveyTypeId);
    Task<List<Survey>> GetActiveSurveysAsync();
} 