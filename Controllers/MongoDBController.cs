using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;
using SurveyApp.Models;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MongoDBController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;

    public MongoDBController(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var surveys = await _mongoDBService.GetAllAsync();
            return Ok(surveys);
        }
        catch (Exception ex)
        {
            return BadRequest($"MongoDB bağlantı hatası: {ex.Message}");
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSurveys()
    {
        try
        {
            var surveys = await _mongoDBService.GetActiveSurveysAsync();
            return Ok(surveys);
        }
        catch (Exception ex)
        {
            return BadRequest($"MongoDB bağlantı hatası: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var survey = await _mongoDBService.GetByIdAsync(id);
            if (survey == null)
                return NotFound("Anket bulunamadı");
            
            return Ok(survey);
        }
        catch (Exception ex)
        {
            return BadRequest($"MongoDB bağlantı hatası: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Survey survey)
    {
        try
        {
            survey.CreatedAt = DateTime.UtcNow;
            await _mongoDBService.CreateAsync(survey);
            return CreatedAtAction(nameof(GetAll), survey);
        }
        catch (Exception ex)
        {
            return BadRequest($"MongoDB bağlantı hatası: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Survey survey)
    {
        try
        {
            await _mongoDBService.UpdateAsync(id, survey);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"MongoDB bağlantı hatası: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"MongoDB bağlantı hatası: {ex.Message}");
        }
    }
} 