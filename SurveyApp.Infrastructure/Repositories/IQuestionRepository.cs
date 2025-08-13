using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IQuestionRepository
{
    Task<List<Question>> GetAllAsync();
    Task<Question?> GetByIdAsync(int id);
    Task<Question> CreateAsync(Question question);
    Task<bool> UpdateAsync(int id, Question question);
    Task<bool> DeleteAsync(int id);
    Task<List<Question>> GetBySurveyIdAsync(int surveyId);
} 