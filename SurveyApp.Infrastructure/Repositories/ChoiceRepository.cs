using MongoDB.Driver;
using MongoDB.Bson;
using SurveyApp.Models;
using SurveyApp.Services;

namespace SurveyApp.Infrastructure.Repositories;

public class ChoiceRepository : IChoiceRepository
{
    private readonly IMongoCollection<Choice> _choices;
    private readonly AutoIncrementService _autoIncrementService;

    public ChoiceRepository(IMongoDatabase database, AutoIncrementService autoIncrementService)
    {
        _choices = database.GetCollection<Choice>("choices");
        _autoIncrementService = autoIncrementService;
    }

    public async Task<List<Choice>> GetAllAsync()
    {
        // Sadece _id türü Int32 olan kayıtları getir (ObjectId olan eski kayıtlar deserialize hatası üretir)
        var typeInt32Filter = (FilterDefinition<Choice>) new BsonDocument("_id", new BsonDocument("$type", 16));
        return await _choices.Find(typeInt32Filter).ToListAsync();
    }

    public async Task<Choice?> GetByIdAsync(int id)
    {
        return await _choices.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Choice> CreateAsync(Choice choice)
    {
        // Otomatik ID ataması (sadece set edilmemişse)
        if (choice.Id == 0)
        {
            choice.Id = await _autoIncrementService.GetNextIdAsync("choices");
        }
        await _choices.InsertOneAsync(choice);
        return choice;
    }

    public async Task<bool> UpdateAsync(int id, Choice choice)
    {
        var result = await _choices.ReplaceOneAsync(c => c.Id == id, choice);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _choices.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Choice>> GetByQuestionIdAsync(int questionId)
    {
        return await _choices.Find(c => c.QuestionsId == questionId).ToListAsync();
    }

    public async Task<bool> DeleteByQuestionIdAsync(int questionId)
    {
        var result = await _choices.DeleteManyAsync(c => c.QuestionsId == questionId);
        return result.DeletedCount > 0;
    }
}
