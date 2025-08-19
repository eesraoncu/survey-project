using MongoDB.Driver;
using MongoDB.Bson;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class QuestionTypeRepository : IQuestionTypeRepository
{
    private readonly IMongoCollection<QuestionType> _questionTypes;
    private readonly AutoIncrementService _autoIncrementService;

    public QuestionTypeRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _questionTypes = database.GetCollection<QuestionType>("question_types");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<QuestionType>> GetAllAsync()
    {
        var typeInt32Filter = (FilterDefinition<QuestionType>) new BsonDocument("_id", new BsonDocument("$type", 16));
        return await _questionTypes.Find(typeInt32Filter).ToListAsync();
    }

    public async Task<QuestionType?> GetByIdAsync(int id)
    {
        return await _questionTypes.Find(qt => qt.Id == id).FirstOrDefaultAsync();
    }

    public async Task<QuestionType> CreateAsync(QuestionType questionType)
    {
        if (questionType.Id == 0)
        {
            questionType.Id = await _autoIncrementService.GetNextIdAsync("question_types");
        }
        await _questionTypes.InsertOneAsync(questionType);
        return questionType;
    }

    public async Task<bool> UpdateAsync(int id, QuestionType questionType)
    {
        var result = await _questionTypes.ReplaceOneAsync(qt => qt.Id == id, questionType);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _questionTypes.DeleteOneAsync(qt => qt.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<QuestionType?> GetByCodeAsync(string questionTypeCode)
    {
        return await _questionTypes.Find(qt => qt.QuestionTypeCode == questionTypeCode).FirstOrDefaultAsync();
    }

    public async Task<List<QuestionType>> GetActiveTypesAsync()
    {
        return await _questionTypes.Find(qt => qt.IsActive == true).ToListAsync();
    }

    public async Task<List<QuestionType>> GetTypesRequiringChoicesAsync()
    {
        return await _questionTypes.Find(qt => qt.RequiresChoices == true && qt.IsActive == true).ToListAsync();
    }
}
