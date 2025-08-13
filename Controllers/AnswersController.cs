using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AnswersController : ControllerBase
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IMapper _mapper;

    public AnswersController(IAnswerRepository answerRepository, IMapper mapper)
    {
        _answerRepository = answerRepository;
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
        var entity = _mapper.Map<Answer>(request);
        var created = await _answerRepository.CreateAsync(entity);
        var response = _mapper.Map<AnswerResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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


