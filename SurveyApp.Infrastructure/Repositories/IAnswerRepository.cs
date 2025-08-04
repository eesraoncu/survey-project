using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IAnswerRepository
{
    Task<List<Answer>> GetAllAsync();
    Task<Answer?> GetByIdAsync(string id);
    Task<List<Answer>> GetByQuestionIdAsync(string questionId);
    Task<List<Answer>> GetBySurveyIdAsync(string surveyId);
    Task<List<Answer>> GetByUserIdAsync(string userId);
    Task<Answer?> GetByQuestionAndUserAsync(string questionId, string userId);
    Task CreateAsync(Answer answer);
    Task UpdateAsync(string id, Answer answer);
    Task DeleteAsync(string id);
    Task DeleteBySurveyIdAsync(string surveyId);
    Task<bool> ExistsAsync(string id);
} 