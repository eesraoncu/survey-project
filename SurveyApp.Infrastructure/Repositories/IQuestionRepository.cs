using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IQuestionRepository
{
    Task<List<Question>> GetAllAsync();
    Task<Question?> GetByIdAsync(string id);
    Task<List<Question>> GetBySurveyIdAsync(string surveyId);
    Task<List<Question>> GetBySurveyIdOrderedAsync(string surveyId);
    Task CreateAsync(Question question);
    Task UpdateAsync(string id, Question question);
    Task DeleteAsync(string id);
    Task DeleteBySurveyIdAsync(string surveyId);
    Task<bool> ExistsAsync(string id);
} 