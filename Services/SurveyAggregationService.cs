using MongoDB.Driver;
using MongoDB.Bson;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class SurveyAggregationService
{
    private readonly IMongoDatabase _database;

    public SurveyAggregationService(IMongoDatabase database)
    {
        _database = database;
    }

    // Survey ile User ve SurveyType bilgilerini birleştir
    public async Task<List<dynamic>> GetSurveysWithUserInfoAsync()
    {
        var surveysCollection = _database.GetCollection<BsonDocument>("surveys");
        var usersCollection = _database.GetCollection<BsonDocument>("users");
        var surveyTypesCollection = _database.GetCollection<BsonDocument>("survey_types");

        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "users_id" },
                { "foreignField", "_id" },
                { "as", "user_info" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "survey_types" },
                { "localField", "survey_type_id" },
                { "foreignField", "_id" },
                { "as", "survey_type_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$user_info")),
            new BsonDocument("$unwind", new BsonDocument("path", "$survey_type_info"))
        };

        return await surveysCollection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
    }

    // User ile Address ve Role bilgilerini birleştir
    public async Task<List<dynamic>> GetUsersWithAddressAndRoleAsync()
    {
        var usersCollection = _database.GetCollection<BsonDocument>("users");
        var addressesCollection = _database.GetCollection<BsonDocument>("addresses");
        var rolesCollection = _database.GetCollection<BsonDocument>("roles");

        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "addresses" },
                { "localField", "address_id" },
                { "foreignField", "_id" },
                { "as", "address_info" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "roles" },
                { "localField", "role_id" },
                { "foreignField", "_id" },
                { "as", "role_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$address_info")),
            new BsonDocument("$unwind", new BsonDocument("path", "$role_info"))
        };

        return await usersCollection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
    }

    // Survey ile Questions ve Choices bilgilerini birleştir
    public async Task<List<dynamic>> GetSurveyWithQuestionsAndChoicesAsync(string surveyId)
    {
        var surveysCollection = _database.GetCollection<BsonDocument>("surveys");
        var questionsCollection = _database.GetCollection<BsonDocument>("questions");
        var choicesCollection = _database.GetCollection<BsonDocument>("choices");

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
            new BsonDocument("$unwind", new BsonDocument("path", "$questions")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "choices" },
                { "localField", "questions._id" },
                { "foreignField", "questions_id" },
                { "as", "questions.choices" }
            })
        };

        return await surveysCollection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
    }

    // Address ile Location Hierarchy bilgilerini birleştir
    public async Task<List<dynamic>> GetAddressWithLocationHierarchyAsync()
    {
        var addressesCollection = _database.GetCollection<BsonDocument>("addresses");
        var neighbourhoodsCollection = _database.GetCollection<BsonDocument>("neighbourhoods");
        var districtTownshipTownsCollection = _database.GetCollection<BsonDocument>("district_township_towns");
        var districtsCollection = _database.GetCollection<BsonDocument>("districts");
        var citiesCollection = _database.GetCollection<BsonDocument>("cities");

        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "neighbourhoods" },
                { "localField", "neighbourhood_id" },
                { "foreignField", "_id" },
                { "as", "neighbourhood_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$neighbourhood_info")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "district_township_towns" },
                { "localField", "neighbourhood_info.district_township_town_id" },
                { "foreignField", "_id" },
                { "as", "district_township_town_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$district_township_town_info")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "districts" },
                { "localField", "district_township_town_info.district_id" },
                { "foreignField", "_id" },
                { "as", "district_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$district_info")),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "cities" },
                { "localField", "district_info.city_id" },
                { "foreignField", "_id" },
                { "as", "city_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$city_info"))
        };

        return await addressesCollection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
    }

    // Survey Responses ile User ve Question bilgilerini birleştir
    public async Task<List<dynamic>> GetSurveyResponsesWithDetailsAsync(string surveyId)
    {
        var answersCollection = _database.GetCollection<BsonDocument>("answers");
        var usersCollection = _database.GetCollection<BsonDocument>("users");
        var questionsCollection = _database.GetCollection<BsonDocument>("questions");

        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument("surveys_id", new BsonObjectId(new ObjectId(surveyId)))),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "users_id" },
                { "foreignField", "_id" },
                { "as", "user_info" }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "questions" },
                { "localField", "questions_id" },
                { "foreignField", "_id" },
                { "as", "question_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$user_info")),
            new BsonDocument("$unwind", new BsonDocument("path", "$question_info"))
        };

        return await answersCollection.Aggregate<dynamic>(pipeline.ToArray()).ToListAsync();
    }
} 