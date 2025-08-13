using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;
using SurveyApp.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;
    private readonly IMongoDatabase _database;

    public TestController(MongoDBService mongoDBService, IMongoDatabase database)
    {
        _mongoDBService = mongoDBService;
        _database = database;
    }

    [HttpGet("connection-test")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Bağlantıyı test et
            var surveys = await _mongoDBService.GetAllAsync();
            
            return Ok(new { 
                message = "MongoDB bağlantısı başarılı!",
                connectionStatus = "Connected",
                documentCount = surveys.Count,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                message = "MongoDB bağlantı hatası!",
                error = ex.Message,
                connectionStatus = "Failed",
                timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost("add-test-data")]
    public async Task<IActionResult> AddTestData()
    {
        try
        {
            var testSurvey = new Survey
            {
                SurveyName = "Test Anketi",
                SurveyDescription = "Bağlantı testi için oluşturuldu",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UsersId = 1, // Test için geçici ID
                SurveyTypeId = 1 // Test için geçici ID
            };

            await _mongoDBService.CreateAsync(testSurvey);

            return Ok(new { 
                message = "Test verisi başarıyla eklendi!",
                survey = testSurvey
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                message = "Test verisi eklenirken hata!",
                error = ex.Message
            });
        }
    }

    [HttpGet("get-all-data")]
    public async Task<IActionResult> GetAllData()
    {
        try
        {
            var surveys = await _mongoDBService.GetAllAsync();
            
            return Ok(new { 
                message = "Tüm veriler başarıyla getirildi!",
                count = surveys.Count,
                surveys = surveys
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                message = "Veri getirme hatası!",
                error = ex.Message
            });
        }
    }

    [HttpPost("update-roles-to-integer")]
    public async Task<IActionResult> UpdateRolesToInteger()
    {
        try
        {
            var rolesCollection = _database.GetCollection<BsonDocument>("roles");
            
            // Mevcut role verilerini sil
            await rolesCollection.DeleteManyAsync(new BsonDocument());
            
            // Yeni integer ID'li role verilerini ekle
            var newRoles = new List<BsonDocument>
            {
                new BsonDocument
                {
                    { "_id", 1 },
                    { "role_name", "admin" },
                    { "created_at", DateTime.UtcNow },
                    { "is_active", true }
                },
                new BsonDocument
                {
                    { "_id", 2 },
                    { "role_name", "user" },
                    { "created_at", DateTime.UtcNow },
                    { "is_active", true }
                },
                new BsonDocument
                {
                    { "_id", 3 },
                    { "role_name", "owner" },
                    { "created_at", DateTime.UtcNow },
                    { "is_active", true }
                }
            };
            
            await rolesCollection.InsertManyAsync(newRoles);
            
            return Ok(new { 
                message = "Role verileri başarıyla güncellendi!",
                roles = newRoles
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                message = "Role güncelleme hatası!",
                error = ex.Message
            });
        }
    }

    [HttpPost("update-users-role-ids")]
    public async Task<IActionResult> UpdateUsersRoleIds()
    {
        try
        {
            var usersCollection = _database.GetCollection<BsonDocument>("users");
            
            // Eski ObjectId'leri yeni integer ID'lere eşle
            var roleIdMappings = new Dictionary<string, int>
            {
                { "688fb686eaf7a08dc53c5c3e", 1 }, // admin
                { "688fb6fdeaf7a08dc53c5c3f", 2 }, // user  
                { "688fb717eaf7a08dc53c5c40", 3 }  // owner
            };
            
            var updateCount = 0;
            foreach (var mapping in roleIdMappings)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("role_id", mapping.Key);
                var update = Builders<BsonDocument>.Update.Set("role_id", mapping.Value);
                var result = await usersCollection.UpdateManyAsync(filter, update);
                updateCount += (int)result.ModifiedCount;
            }
            
            return Ok(new { 
                message = "User role ID'leri başarıyla güncellendi!",
                updatedCount = updateCount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                message = "User role ID güncelleme hatası!",
                error = ex.Message
            });
        }
    }
} 