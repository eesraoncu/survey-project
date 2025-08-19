using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IQuestionTypeRepository
{
    Task<List<QuestionType>> GetAllAsync();
    Task<QuestionType?> GetByIdAsync(int id);
    Task<QuestionType> CreateAsync(QuestionType questionType);
    Task<bool> UpdateAsync(int id, QuestionType questionType);
    Task<bool> DeleteAsync(int id);
    Task<QuestionType?> GetByCodeAsync(string questionTypeCode);
    Task<List<QuestionType>> GetActiveTypesAsync();
    Task<List<QuestionType>> GetTypesRequiringChoicesAsync();
}
