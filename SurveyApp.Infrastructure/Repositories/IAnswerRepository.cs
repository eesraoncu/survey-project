using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IAnswerRepository
{
    Task<List<Answer>> GetAllAsync();
    Task<Answer?> GetByIdAsync(int id);
    Task<Answer> CreateAsync(Answer answer);
    Task<bool> UpdateAsync(int id, Answer answer);
    Task<bool> DeleteAsync(int id);
    Task<List<Answer>> GetByQuestionIdAsync(int questionId);
    Task<List<Answer>> GetByUserIdAsync(int userId);
    Task<List<Answer>> GetBySurveyIdAsync(int surveyId);
} 