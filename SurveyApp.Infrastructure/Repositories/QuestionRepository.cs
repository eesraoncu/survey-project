using MongoDB.Driver;
using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly IMongoCollection<Question> _questions;

    public QuestionRepository(IMongoDatabase database)
    {
        _questions = database.GetCollection<Question>("questions");
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _questions.Find(_ => true).ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(string id)
    {
        return await _questions.Find(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Question> CreateAsync(Question question)
    {
        await _questions.InsertOneAsync(question);
        return question;
    }

    public async Task<bool> UpdateAsync(string id, Question question)
    {
        var result = await _questions.ReplaceOneAsync(q => q.Id == id, question);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _questions.DeleteOneAsync(q => q.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Question>> GetBySurveyIdAsync(string surveyId)
    {
        return await _questions.Find(q => q.SurveysId == surveyId).ToListAsync();
    }
} 