using MongoDB.Driver;
using MongoDB.Bson;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class SurveyAggregationService
{
    private readonly IMongoDatabase _database;

    public SurveyAggregationService(IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
        
        if (mongoSettings == null)
            throw new ArgumentNullException(nameof(mongoSettings), "MongoDB ayarları bulunamadı!");

        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        var client = new MongoClient(settings);
        _database = client.GetDatabase(mongoSettings.DatabaseName);
    }

    // Survey ile User bilgilerini birleştirme
    public async Task<List<dynamic>> GetSurveysWithUserInfoAsync()
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "users_id" },
                { "foreignField", "_id" },
                { "as", "user" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "survey_types" },
                { "localField", "survey_type_id" },
                { "foreignField", "_id" },
                { "as", "survey_type" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$user")),
            new BsonDocument("$unwind", new BsonDocument("path", "$survey_type"))
        };

        var collection = _database.GetCollection<BsonDocument>("surveys");
        var result = await collection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
        return result;
    }

    // User ile Address ve Role bilgilerini birleştirme
    public async Task<List<dynamic>> GetUsersWithAddressAndRoleAsync()
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "addresses" },
                { "localField", "address_id" },
                { "foreignField", "_id" },
                { "as", "address" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "roles" },
                { "localField", "role_id" },
                { "foreignField", "_id" },
                { "as", "role" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$address")),
            new BsonDocument("$unwind", new BsonDocument("path", "$role"))
        };

        var collection = _database.GetCollection<BsonDocument>("users");
        var result = await collection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
        return result;
    }

    // Survey ile Questions ve Choices bilgilerini birleştirme
    public async Task<List<dynamic>> GetSurveyWithQuestionsAndChoicesAsync(string surveyId)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument("_id", new BsonObjectId(new ObjectId(surveyId)))),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "questions" },
                { "localField", "_id" },
                { "foreignField", "surveys_id" },
                { "as", "questions" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "choices" },
                { "localField", "questions._id" },
                { "foreignField", "questions_id" },
                { "as", "choices" }
            })
        };

        var collection = _database.GetCollection<BsonDocument>("surveys");
        var result = await collection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
        return result;
    }

    // Address ile Location hierarchy bilgilerini birleştirme
    public async Task<List<dynamic>> GetAddressWithLocationHierarchyAsync()
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "neighbourhoods" },
                { "localField", "neighbourhood_id" },
                { "foreignField", "_id" },
                { "as", "neighbourhood" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$neighbourhood")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "district_township_towns" },
                { "localField", "neighbourhood.district_township_town_id" },
                { "foreignField", "_id" },
                { "as", "district_township_town" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$district_township_town")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "districts" },
                { "localField", "district_township_town.district_id" },
                { "foreignField", "_id" },
                { "as", "district" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$district")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "cities" },
                { "localField", "district.city_id" },
                { "foreignField", "_id" },
                { "as", "city" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$city"))
        };

        var collection = _database.GetCollection<BsonDocument>("addresses");
        var result = await collection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
        return result;
    }

    // Survey responses ile User ve Question bilgilerini birleştirme
    public async Task<List<dynamic>> GetSurveyResponsesWithDetailsAsync(string surveyId)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument("surveys_id", new BsonObjectId(new ObjectId(surveyId)))),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "users_id" },
                { "foreignField", "_id" },
                { "as", "user" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "questions" },
                { "localField", "questions_id" },
                { "foreignField", "_id" },
                { "as", "question" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$user")),
            new BsonDocument("$unwind", new BsonDocument("path", "$question"))
        };

        var collection = _database.GetCollection<BsonDocument>("answers");
        var result = await collection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
        return result;
    }
} 