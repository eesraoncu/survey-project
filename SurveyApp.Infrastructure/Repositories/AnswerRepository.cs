using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly IMongoCollection<Answer> _answers;

    public AnswerRepository(IMongoDatabase database)
    {
        _answers = database.GetCollection<Answer>("answers");
    }

    public async Task<List<Answer>> GetAllAsync()
    {
        return await _answers.Find(_ => true).ToListAsync();
    }

    public async Task<Answer?> GetByIdAsync(string id)
    {
        return await _answers.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Answer> CreateAsync(Answer answer)
    {
        await _answers.InsertOneAsync(answer);
        return answer;
    }

    public async Task<bool> UpdateAsync(string id, Answer answer)
    {
        var result = await _answers.ReplaceOneAsync(a => a.Id == id, answer);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _answers.DeleteOneAsync(a => a.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Answer>> GetByQuestionIdAsync(string questionId)
    {
        return await _answers.Find(a => a.QuestionsId == questionId).ToListAsync();
    }

    public async Task<List<Answer>> GetByUserIdAsync(string userId)
    {
        return await _answers.Find(a => a.UsersId == userId).ToListAsync();
    }

    public async Task<List<Answer>> GetBySurveyIdAsync(string surveyId)
    {
        return await _answers.Find(a => a.SurveysId == surveyId).ToListAsync();
    }
} 