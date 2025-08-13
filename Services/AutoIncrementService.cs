using MongoDB.Driver;
using MongoDB.Bson;

namespace SurveyApp.Services;

public class AutoIncrementService
{
    private readonly IMongoDatabase _database;

    public AutoIncrementService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<int> GetNextIdAsync(string collectionName)
    {
        var countersCollection = _database.GetCollection<BsonDocument>("counters");
        
        var filter = Builders<BsonDocument>.Filter.Eq("_id", collectionName);
        var update = Builders<BsonDocument>.Update.Inc("seq", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var result = await countersCollection.FindOneAndUpdateAsync(filter, update, options);
        return result["seq"].AsInt32;
    }
}
