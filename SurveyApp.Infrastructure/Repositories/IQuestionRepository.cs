using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IQuestionRepository
{
    Task<List<Question>> GetAllAsync();
    Task<Question?> GetByIdAsync(string id);
    Task<Question> CreateAsync(Question question);
    Task<bool> UpdateAsync(string id, Question question);
    Task<bool> DeleteAsync(string id);
    Task<List<Question>> GetBySurveyIdAsync(string surveyId);
} 