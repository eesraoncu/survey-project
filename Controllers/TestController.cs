using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;
using SurveyApp.Models;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;

    public TestController(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
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
                UsersId = "test_user_id", // Test için geçici ID
                SurveyTypeId = "test_survey_type_id" // Test için geçici ID
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
} 