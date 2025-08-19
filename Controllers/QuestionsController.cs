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
    private readonly IChoiceRepository _choiceRepository;
    private readonly IQuestionTypeRepository _questionTypeRepository;
    private readonly IMapper _mapper;

    public QuestionsController(IQuestionRepository questionRepository, IChoiceRepository choiceRepository, IQuestionTypeRepository questionTypeRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _choiceRepository = choiceRepository;
        _questionTypeRepository = questionTypeRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionResponse>>> GetAll()
    {
        var list = await _questionRepository.GetAllAsync();
        return Ok(_mapper.Map<List<QuestionResponse>>(list));
    }

    [HttpGet("types")]
    public async Task<ActionResult<List<QuestionTypeResponse>>> GetQuestionTypes()
    {
        var types = await _questionTypeRepository.GetActiveTypesAsync();
        var responses = _mapper.Map<List<QuestionTypeResponse>>(types);
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionResponse>> GetById(int id)
    {
        var entity = await _questionRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        
        var response = _mapper.Map<QuestionResponse>(entity);
        
        // Get question type details
        if (entity.QuestionTypeId > 0)
        {
            var questionType = await _questionTypeRepository.GetByIdAsync(entity.QuestionTypeId);
            if (questionType != null)
            {
                response.QuestionTypeDetails = _mapper.Map<QuestionTypeResponse>(questionType);
                response.TypeDisplayName = questionType.QuestionTypeName;
                response.RequiresChoices = questionType.RequiresChoices;
                response.AllowsMultipleSelection = questionType.AllowsMultipleSelection;
            }
        }
        
        // Get choices for this question
        var choices = await _choiceRepository.GetByQuestionIdAsync(id);
        response.ChoiceOptions = _mapper.Map<List<ChoiceResponse>>(choices);
        
        return Ok(response);
    }

    [HttpGet("by-survey/{surveyId}")]
    public async Task<ActionResult<List<QuestionResponse>>> GetBySurveyId(int surveyId)
    {
        var list = await _questionRepository.GetBySurveyIdAsync(surveyId);
        var responses = _mapper.Map<List<QuestionResponse>>(list);
        
        // Get question type details and choices for each question
        foreach (var response in responses)
        {
            var entity = list.First(q => q.Id == response.Id);
            
            // Get question type details
            if (entity.QuestionTypeId > 0)
            {
                var questionType = await _questionTypeRepository.GetByIdAsync(entity.QuestionTypeId);
                if (questionType != null)
                {
                    response.QuestionTypeDetails = _mapper.Map<QuestionTypeResponse>(questionType);
                    response.TypeDisplayName = questionType.QuestionTypeName;
                    response.RequiresChoices = questionType.RequiresChoices;
                    response.AllowsMultipleSelection = questionType.AllowsMultipleSelection;
                }
            }
            
            // Get choices for this question
            var choices = await _choiceRepository.GetByQuestionIdAsync(response.Id);
            response.ChoiceOptions = _mapper.Map<List<ChoiceResponse>>(choices);
        }
        
        return Ok(responses);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionResponse>> Create([FromBody] QuestionCreateRequest request, [FromQuery] int? surveyId)
    {
        var incomingText = string.IsNullOrWhiteSpace(request.QuestionsText) ? request.QuestionText : request.QuestionsText;
        Console.WriteLine($"[QuestionsController.Create] Incoming => Text='{incomingText}', Type='{request.QuestionType}', QuestionTypeId={request.QuestionTypeId}, SurveysId={request.SurveysId}, alt SurveyId={request.SurveyId}, query surveyId={surveyId}");

        // SurveysId zorunlu: gövdeden gelmediyse query veya route üzerinden almayı dene
        var effectiveSurveyId = request.SurveysId > 0 ? request.SurveysId : (request.SurveyId ?? surveyId ?? 0);
        if (effectiveSurveyId <= 0)
        {
            return BadRequest(new { message = "SurveysId gerekli. Gövdede 'surveysId' ya da query'de 'surveyId' vermelisiniz." });
        }

        // QuestionTypeId kontrolü
        var effectiveQuestionTypeId = request.QuestionTypeId;
        if (effectiveQuestionTypeId <= 0)
        {
            return BadRequest(new { message = "QuestionTypeId gerekli." });
        }

        // Question type'ın var olup olmadığını kontrol et
        var questionType = await _questionTypeRepository.GetByIdAsync(effectiveQuestionTypeId);
        if (questionType == null)
        {
            return BadRequest(new { message = $"Question type with ID {effectiveQuestionTypeId} not found." });
        }

        var entity = _mapper.Map<Question>(request);
        entity.SurveysId = effectiveSurveyId;
        entity.QuestionsText = incomingText ?? string.Empty;
        entity.QuestionTypeId = effectiveQuestionTypeId;
        entity.QuestionType = questionType.QuestionTypeCode;
        entity.UpdatedAt = DateTime.UtcNow;

        var created = await _questionRepository.CreateAsync(entity);
        var response = _mapper.Map<QuestionResponse>(created);
        
        // Response'a question type detaylarını ekle
        response.QuestionTypeDetails = _mapper.Map<QuestionTypeResponse>(questionType);
        response.TypeDisplayName = questionType.QuestionTypeName;
        response.RequiresChoices = questionType.RequiresChoices;
        response.AllowsMultipleSelection = questionType.AllowsMultipleSelection;
        
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


