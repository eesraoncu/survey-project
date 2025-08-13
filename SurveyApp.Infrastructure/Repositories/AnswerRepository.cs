using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly IMongoCollection<Answer> _answers;
    private readonly AutoIncrementService _autoIncrementService;

    public AnswerRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _answers = database.GetCollection<Answer>("answers");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<Answer>> GetAllAsync()
    {
        return await _answers.Find(_ => true).ToListAsync();
    }

    public async Task<Answer?> GetByIdAsync(int id)
    {
        return await _answers.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Answer> CreateAsync(Answer answer)
    {
        // Otomatik ID ataması
        answer.Id = await _autoIncrementService.GetNextIdAsync("answers");
        await _answers.InsertOneAsync(answer);
        return answer;
    }

    public async Task<bool> UpdateAsync(int id, Answer answer)
    {
        var result = await _answers.ReplaceOneAsync(a => a.Id == id, answer);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _answers.DeleteOneAsync(a => a.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Answer>> GetByQuestionIdAsync(int questionId)
    {
        return await _answers.Find(a => a.QuestionsId == questionId).ToListAsync();
    }

    public async Task<List<Answer>> GetByUserIdAsync(int userId)
    {
        return await _answers.Find(a => a.UsersId == userId).ToListAsync();
    }

    public async Task<List<Answer>> GetBySurveyIdAsync(int surveyId)
    {
        return await _answers.Find(a => a.SurveysId == surveyId).ToListAsync();
    }
} 