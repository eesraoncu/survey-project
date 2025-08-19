using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using SurveyApp.Services;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AnswersController : ControllerBase
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AnswersController(IAnswerRepository answerRepository, IUserService userService, IMapper mapper)
    {
        _answerRepository = answerRepository;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnswerResponse>>> GetAll()
    {
        var list = await _answerRepository.GetAllAsync();
        return Ok(_mapper.Map<List<AnswerResponse>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnswerResponse>> GetById(int id)
    {
        var entity = await _answerRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<AnswerResponse>(entity));
    }

    [HttpGet("by-question/{questionId}")]
    public async Task<ActionResult<List<AnswerResponse>>> GetByQuestionId(int questionId)
    {
        var list = await _answerRepository.GetByQuestionIdAsync(questionId);
        return Ok(_mapper.Map<List<AnswerResponse>>(list));
    }

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<List<AnswerResponse>>> GetByUserId(int userId)
    {
        var list = await _answerRepository.GetByUserIdAsync(userId);
        return Ok(_mapper.Map<List<AnswerResponse>>(list));
    }

    [HttpGet("by-survey/{surveyId}")]
    public async Task<ActionResult<List<AnswerResponse>>> GetBySurveyId(int surveyId)
    {
        var list = await _answerRepository.GetBySurveyIdAsync(surveyId);
        return Ok(_mapper.Map<List<AnswerResponse>>(list));
    }

    [HttpPost]
    public async Task<ActionResult<AnswerResponse>> Create([FromBody] AnswerCreateRequest request)
    {
        // Frontend uyumluluğu için alternatif field'ları kontrol et
        var effectiveUserId = request.UsersId > 0 ? request.UsersId : (request.UserId ?? 0);
        var effectiveSurveyId = request.SurveysId > 0 ? request.SurveysId : (request.SurveyId ?? 0);
        var effectiveQuestionId = request.QuestionsId > 0 ? request.QuestionsId : (request.QuestionId ?? 0);

        if (effectiveUserId <= 0)
            return BadRequest(new { message = "UsersId is required." });
        if (effectiveSurveyId <= 0)
            return BadRequest(new { message = "SurveysId is required." });
        if (effectiveQuestionId <= 0)
            return BadRequest(new { message = "QuestionsId is required." });

        // Anket cevaplama işlemi - kullanıcının user rolünde olduğundan emin ol
        await _userService.EnsureUserRoleForAnsweringAsync(effectiveUserId, effectiveSurveyId);
        
        var entity = _mapper.Map<Answer>(request);
        entity.UsersId = effectiveUserId;
        entity.SurveysId = effectiveSurveyId;
        entity.QuestionsId = effectiveQuestionId;
        entity.UpdatedAt = DateTime.UtcNow;

        var created = await _answerRepository.CreateAsync(entity);
        var response = _mapper.Map<AnswerResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<AnswerResponse>>> CreateBulk([FromBody] BulkAnswerCreateRequest request)
    {
        // Frontend uyumluluğu için alternatif field'ları kontrol et
        var effectiveUserId = request.UsersId > 0 ? request.UsersId : (request.UserId ?? 0);
        var effectiveSurveyId = request.SurveysId > 0 ? request.SurveysId : (request.SurveyId ?? 0);

        if (effectiveUserId <= 0)
            return BadRequest(new { message = "UsersId is required." });
        if (effectiveSurveyId <= 0)
            return BadRequest(new { message = "SurveysId is required." });

        // Anket cevaplama işlemi - kullanıcının user rolünde olduğundan emin ol
        await _userService.EnsureUserRoleForAnsweringAsync(effectiveUserId, effectiveSurveyId);

        var responses = new List<AnswerResponse>();
        foreach (var answerRequest in request.Answers)
        {
            if (answerRequest.QuestionsId > 0 || answerRequest.QuestionId > 0)
            {
                var effectiveQuestionId = answerRequest.QuestionsId > 0 ? answerRequest.QuestionsId : (answerRequest.QuestionId ?? 0);
                
                var entity = _mapper.Map<Answer>(answerRequest);
                entity.UsersId = effectiveUserId;
                entity.SurveysId = effectiveSurveyId;
                entity.QuestionsId = effectiveQuestionId;
                entity.UpdatedAt = DateTime.UtcNow;

                var created = await _answerRepository.CreateAsync(entity);
                responses.Add(_mapper.Map<AnswerResponse>(created));
            }
        }

        return Ok(responses);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AnswerUpdateRequest request)
    {
        var existing = await _answerRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        _mapper.Map(request, existing);
        var ok = await _answerRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _answerRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}


