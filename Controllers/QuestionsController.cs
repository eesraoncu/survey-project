using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuestionsController(IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionResponse>>> GetAll()
    {
        var list = await _questionRepository.GetAllAsync();
        return Ok(_mapper.Map<List<QuestionResponse>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionResponse>> GetById(int id)
    {
        var entity = await _questionRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<QuestionResponse>(entity));
    }

    [HttpGet("by-survey/{surveyId}")]
    public async Task<ActionResult<List<QuestionResponse>>> GetBySurveyId(int surveyId)
    {
        var list = await _questionRepository.GetBySurveyIdAsync(surveyId);
        return Ok(_mapper.Map<List<QuestionResponse>>(list));
    }

    [HttpPost]
    public async Task<ActionResult<QuestionResponse>> Create([FromBody] QuestionCreateRequest request, [FromQuery] int? surveyId)
    {
        var incomingText = string.IsNullOrWhiteSpace(request.QuestionsText) ? request.QuestionText : request.QuestionsText;
        Console.WriteLine($"[QuestionsController.Create] Incoming => Text='{incomingText}', Type='{request.QuestionType}', SurveysId={request.SurveysId}, alt SurveyId={request.SurveyId}, query surveyId={surveyId}");

        // SurveysId zorunlu: gövdeden gelmediyse query veya route üzerinden almayı dene
        var effectiveSurveyId = request.SurveysId > 0 ? request.SurveysId : (request.SurveyId ?? surveyId ?? 0);
        if (effectiveSurveyId <= 0)
        {
            return BadRequest(new { message = "SurveysId gerekli. Gövdede 'surveysId' ya da query'de 'surveyId' vermelisiniz." });
        }

        var entity = _mapper.Map<Question>(request);
        entity.SurveysId = effectiveSurveyId;
        entity.QuestionsText = incomingText ?? string.Empty;

        var created = await _questionRepository.CreateAsync(entity);
        var response = _mapper.Map<QuestionResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    // Frontend kolaylığı: /api/Questions/by-survey/{surveyId} üzerinden POST ile soru ekleme
    [HttpPost("by-survey/{surveyId}")]
    public async Task<ActionResult<QuestionResponse>> CreateBySurvey(int surveyId, [FromBody] QuestionCreateRequest request)
    {
        request.SurveysId = surveyId;
        return await Create(request, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] QuestionUpdateRequest request)
    {
        var existing = await _questionRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        _mapper.Map(request, existing);
        var ok = await _questionRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _questionRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}


