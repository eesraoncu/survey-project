using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SurveyApp.Services;
using SurveyApp.Models;
using System.Security.Claims;
using AutoMapper;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IMapper _mapper;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IQuestionRepository _questionRepository;

    public AIController(IAIService aiService, IMapper mapper, ISurveyRepository surveyRepository, IQuestionRepository questionRepository)
    {
        _aiService = aiService;
        _mapper = mapper;
        _surveyRepository = surveyRepository;
        _questionRepository = questionRepository;
    }

    [HttpPost("analyze-survey/{surveyId}")]
    public async Task<ActionResult<AIAnalysisResult>> AnalyzeSurvey(int surveyId, [FromQuery] string analysisType = "general")
    {
        try
        {
            var result = await _aiService.AnalyzeSurveyResultsAsync(surveyId, analysisType);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "AI analysis failed", error = ex.Message });
        }
    }

    [HttpPost("sentiment-analysis")]
    public async Task<ActionResult<SentimentAnalysis>> AnalyzeSentiment([FromBody] List<string> responses)
    {
        try
        {
            if (responses == null || !responses.Any())
                return BadRequest(new { message = "No responses provided" });

            var result = await _aiService.AnalyzeSentimentAsync(responses);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sentiment analysis failed", error = ex.Message });
        }
    }

    [HttpGet("insights/{surveyId}")]
    public async Task<ActionResult<List<SurveyInsight>>> GenerateInsights(int surveyId)
    {
        try
        {
            var insights = await _aiService.GenerateInsightsAsync(surveyId);
            return Ok(insights);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Insight generation failed", error = ex.Message });
        }
    }

    [HttpGet("summary/{surveyId}")]
    public async Task<ActionResult<string>> GenerateSummary(int surveyId)
    {
        try
        {
            var summary = await _aiService.GenerateSummaryAsync(surveyId);
            return Ok(new { summary });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Summary generation failed", error = ex.Message });
        }
    }

    [HttpPost("generate-questions")]
    public async Task<ActionResult<List<SmartQuestion>>> GenerateSmartQuestions([FromBody] QuestionSuggestionRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.SurveyTitle))
                return BadRequest(new { message = "Survey title is required" });

            var questions = await _aiService.GenerateSmartQuestionsAsync(request);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Question generation failed", error = ex.Message });
        }
    }

    [HttpPost("suggest-choices")]
    public async Task<ActionResult<List<string>>> SuggestAnswerChoices([FromBody] ChoiceSuggestionRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.QuestionText))
                return BadRequest(new { message = "Question text is required" });

            var choices = await _aiService.SuggestAnswerChoicesAsync(request.QuestionText, request.QuestionType);
            return Ok(choices);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Choice suggestion failed", error = ex.Message });
        }
    }

    [HttpPost("improve-question")]
    public async Task<ActionResult<string>> ImproveQuestion([FromBody] QuestionImprovementRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.QuestionText))
                return BadRequest(new { message = "Question text is required" });

            var improvedQuestion = await _aiService.ImproveQuestionTextAsync(request.QuestionText);
            return Ok(new { improvedQuestion });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Question improvement failed", error = ex.Message });
        }
    }

    [HttpPost("extract-keywords")]
    public async Task<ActionResult<List<string>>> ExtractKeywords([FromBody] KeywordExtractionRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Text))
                return BadRequest(new { message = "Text is required" });

            var keywords = await _aiService.ExtractKeywordsAsync(request.Text);
            return Ok(keywords);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Keyword extraction failed", error = ex.Message });
        }
    }

    [HttpPost("translate")]
    public async Task<ActionResult<string>> TranslateText([FromBody] TranslationRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.TargetLanguage))
                return BadRequest(new { message = "Text and target language are required" });

            var translatedText = await _aiService.TranslateTextAsync(request.Text, request.TargetLanguage);
            return Ok(new { translatedText });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Translation failed", error = ex.Message });
        }
    }

    [HttpGet("report/{surveyId}")]
    public async Task<ActionResult<string>> GenerateReport(int surveyId, [FromQuery] string reportType = "comprehensive")
    {
        try
        {
            var report = await _aiService.GenerateReportAsync(surveyId, reportType);
            return Ok(new { report });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Report generation failed", error = ex.Message });
        }
    }

    [HttpPost("generate-complete-survey")]
    public async Task<ActionResult<Survey>> GenerateCompleteSurvey([FromBody] SurveyGenerationRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Description))
                return BadRequest(new { message = "Survey description is required" });

            // Authentication yoksa default user ID kullan
            var currentUserId = GetCurrentUserIdOrDefault();
            var survey = await _aiService.GenerateCompleteSurveyAsync(request.Description, currentUserId);
            
            // Anket oluşturulduktan sonra questions'larla birlikte tam response döndür
            // SurveysController'daki GetById'ı çağırarak questions'larla birlikte döndür
            return await GetSurveyWithQuestions(survey.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Complete survey generation failed", error = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return userId;
        
        throw new InvalidOperationException("User ID not found in claims");
    }

    private int GetCurrentUserIdOrDefault()
    {
        try
        {
            return GetCurrentUserId();
        }
        catch
        {
            // Authentication yoksa default user ID döndür
            return 1; // Default admin user ID
        }
    }

    private async Task<ActionResult> GetSurveyWithQuestions(int surveyId)
    {
        try
        {
            // SurveysController'daki GetById mantığını burada da kullanalım
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null) 
                return NotFound(new { message = "Survey not found" });

            // Survey'i SurveyResponse'a map et
            var surveyResponse = _mapper.Map<SurveyResponse>(survey);
            
            // Questions'ları getir
            var questions = await _questionRepository.GetBySurveyIdAsync(surveyId);
            surveyResponse.Questions = _mapper.Map<List<QuestionResponse>>(questions);
            
            return CreatedAtAction("GetById", "Surveys", new { id = survey.Id }, surveyResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving survey with questions", error = ex.Message });
        }
    }
}

// Request DTOs
public class ChoiceSuggestionRequest
{
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
}

public class QuestionImprovementRequest
{
    public string QuestionText { get; set; } = string.Empty;
}

public class KeywordExtractionRequest
{
    public string Text { get; set; } = string.Empty;
}

public class TranslationRequest
{
    public string Text { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
}

public class SurveyGenerationRequest
{
    public string Description { get; set; } = string.Empty;
}
