using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _questions;
    private readonly AutoIncrementService _autoIncrementService;

    public QuestionRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _questions = database.GetCollection<Question>("questions");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _questions.Find(_ => true).ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _questions.Find(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Question> CreateAsync(Question question)
    {
        // Otomatik ID ataması
        question.Id = await _autoIncrementService.GetNextIdAsync("questions");
        await _questions.InsertOneAsync(question);
        return question;
    }

    public async Task<bool> UpdateAsync(int id, Question question)
    {
        var result = await _questions.ReplaceOneAsync(q => q.Id == id, question);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _questions.DeleteOneAsync(q => q.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Question>> GetBySurveyIdAsync(int surveyId)
    {
        return await _questions.Find(q => q.SurveysId == surveyId).ToListAsync();
    }
} 