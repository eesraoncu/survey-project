using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IAnswerRepository
{
    Task<List<Answer>> GetAllAsync();
    Task<Answer?> GetByIdAsync(string id);
    Task<Answer> CreateAsync(Answer answer);
    Task<bool> UpdateAsync(string id, Answer answer);
    Task<bool> DeleteAsync(string id);
    Task<List<Answer>> GetByQuestionIdAsync(string questionId);
    Task<List<Answer>> GetByUserIdAsync(string userId);
    Task<List<Answer>> GetBySurveyIdAsync(string surveyId);
} 