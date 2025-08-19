using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IChoiceRepository
{
    Task<List<Choice>> GetAllAsync();
    Task<Choice?> GetByIdAsync(int id);
    Task<Choice> CreateAsync(Choice choice);
    Task<bool> UpdateAsync(int id, Choice choice);
    Task<bool> DeleteAsync(int id);
    Task<List<Choice>> GetByQuestionIdAsync(int questionId);
    Task<bool> DeleteByQuestionIdAsync(int questionId);
}
