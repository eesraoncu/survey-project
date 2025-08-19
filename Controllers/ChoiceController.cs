using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ChoiceController : ControllerBase
{
    private readonly IChoiceRepository _choiceRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public ChoiceController(IChoiceRepository choiceRepository, IQuestionRepository questionRepository, IMapper mapper)
    {
        _choiceRepository = choiceRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ChoiceResponse>>> GetAll()
    {
        var list = await _choiceRepository.GetAllAsync();
        return Ok(_mapper.Map<List<ChoiceResponse>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChoiceResponse>> GetById(int id)
    {
        var entity = await _choiceRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<ChoiceResponse>(entity));
    }

    [HttpGet("by-question/{questionId}")]
    public async Task<ActionResult<List<ChoiceResponse>>> GetByQuestionId(int questionId)
    {
        var list = await _choiceRepository.GetByQuestionIdAsync(questionId);
        return Ok(_mapper.Map<List<ChoiceResponse>>(list));
    }

    [HttpPost]
    public async Task<ActionResult<ChoiceResponse>> Create([FromBody] ChoiceCreateRequest request)
    {
        var effectiveQuestionId = request.QuestionsId > 0 ? request.QuestionsId : (request.QuestionId ?? 0);
        if (effectiveQuestionId <= 0)
        {
            return BadRequest(new { message = "QuestionsId is required." });
        }

        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(effectiveQuestionId);
        if (question == null)
        {
            return BadRequest(new { message = $"Question with ID {effectiveQuestionId} not found." });
        }

        var entity = _mapper.Map<Choice>(request);
        entity.QuestionsId = effectiveQuestionId;

        var created = await _choiceRepository.CreateAsync(entity);
        var response = _mapper.Map<ChoiceResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<ChoiceResponse>>> CreateBulk([FromBody] BulkChoiceCreateRequest request)
    {
        var effectiveQuestionId = request.QuestionsId > 0 ? request.QuestionsId : (request.QuestionId ?? 0);
        if (effectiveQuestionId <= 0)
        {
            return BadRequest(new { message = "QuestionsId is required." });
        }

        // Verify question exists
        var question = await _questionRepository.GetByIdAsync(effectiveQuestionId);
        if (question == null)
        {
            return BadRequest(new { message = $"Question with ID {effectiveQuestionId} not found." });
        }

        var responses = new List<ChoiceResponse>();
        foreach (var choiceText in request.ChoiceTexts)
        {
            if (!string.IsNullOrWhiteSpace(choiceText))
            {
                var choice = new Choice
                {
                    ChoiceText = choiceText.Trim(),
                    QuestionsId = effectiveQuestionId
                };
                var created = await _choiceRepository.CreateAsync(choice);
                responses.Add(_mapper.Map<ChoiceResponse>(created));
            }
        }

        return Ok(responses);
    }

    [HttpPost("by-question/{questionId}")]
    public async Task<ActionResult<ChoiceResponse>> CreateByQuestion(int questionId, [FromBody] ChoiceCreateRequest request)
    {
        request.QuestionsId = questionId;
        return await Create(request);
    }

    [HttpPost("bulk/by-question/{questionId}")]
    public async Task<ActionResult<List<ChoiceResponse>>> CreateBulkByQuestion(int questionId, [FromBody] BulkChoiceCreateRequest request)
    {
        request.QuestionsId = questionId;
        return await CreateBulk(request);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ChoiceUpdateRequest request)
    {
        var existing = await _choiceRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        // Verify question exists if changed
        if (request.QuestionsId != existing.QuestionsId)
        {
            var question = await _questionRepository.GetByIdAsync(request.QuestionsId);
            if (question == null)
            {
                return BadRequest(new { message = $"Question with ID {request.QuestionsId} not found." });
            }
        }

        _mapper.Map(request, existing);
        var ok = await _choiceRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _choiceRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("by-question/{questionId}")]
    public async Task<IActionResult> DeleteByQuestionId(int questionId)
    {
        var ok = await _choiceRepository.DeleteByQuestionIdAsync(questionId);
        return ok ? NoContent() : NotFound();
    }
}
