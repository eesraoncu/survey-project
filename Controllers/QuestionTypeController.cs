using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QuestionTypeController : ControllerBase
{
    private readonly IQuestionTypeRepository _questionTypeRepository;
    private readonly IMapper _mapper;

    public QuestionTypeController(IQuestionTypeRepository questionTypeRepository, IMapper mapper)
    {
        _questionTypeRepository = questionTypeRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionTypeResponse>>> GetAll()
    {
        var list = await _questionTypeRepository.GetAllAsync();
        return Ok(_mapper.Map<List<QuestionTypeResponse>>(list));
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<QuestionTypeResponse>>> GetActiveTypes()
    {
        var list = await _questionTypeRepository.GetActiveTypesAsync();
        return Ok(_mapper.Map<List<QuestionTypeResponse>>(list));
    }

    [HttpGet("requiring-choices")]
    public async Task<ActionResult<List<QuestionTypeResponse>>> GetTypesRequiringChoices()
    {
        var list = await _questionTypeRepository.GetTypesRequiringChoicesAsync();
        return Ok(_mapper.Map<List<QuestionTypeResponse>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionTypeResponse>> GetById(int id)
    {
        var entity = await _questionTypeRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<QuestionTypeResponse>(entity));
    }

    [HttpGet("by-code/{code}")]
    public async Task<ActionResult<QuestionTypeResponse>> GetByCode(string code)
    {
        var entity = await _questionTypeRepository.GetByCodeAsync(code);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<QuestionTypeResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<QuestionTypeResponse>> Create([FromBody] QuestionTypeCreateRequest request)
    {
        // Check if question type with this code already exists
        var existing = await _questionTypeRepository.GetByCodeAsync(request.QuestionTypeCode);
        if (existing != null)
        {
            return BadRequest(new { message = $"Question type with code '{request.QuestionTypeCode}' already exists." });
        }

        var entity = _mapper.Map<QuestionType>(request);
        var created = await _questionTypeRepository.CreateAsync(entity);
        var response = _mapper.Map<QuestionTypeResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] QuestionTypeUpdateRequest request)
    {
        var existing = await _questionTypeRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        // Check if another question type with this code exists
        var codeExists = await _questionTypeRepository.GetByCodeAsync(request.QuestionTypeCode);
        if (codeExists != null && codeExists.Id != id)
        {
            return BadRequest(new { message = $"Question type with code '{request.QuestionTypeCode}' already exists." });
        }

        _mapper.Map(request, existing);
        var ok = await _questionTypeRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _questionTypeRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("seed-default-types")]
    public async Task<IActionResult> SeedDefaultQuestionTypes()
    {
        var defaultTypes = new[]
        {
            new { Name = "Kısa Yanıt", Code = "short_text", RequiresChoices = false, AllowsMultiple = false },
            new { Name = "Paragraf", Code = "paragraph", RequiresChoices = false, AllowsMultiple = false },
            new { Name = "Çoktan Seçmeli", Code = "multiple_choice", RequiresChoices = true, AllowsMultiple = false },
            new { Name = "Çoklu Seçim", Code = "multi_select", RequiresChoices = true, AllowsMultiple = true },
            new { Name = "Açılır Liste", Code = "dropdown", RequiresChoices = true, AllowsMultiple = false }
        };

        var createdCount = 0;
        foreach (var type in defaultTypes)
        {
            var existing = await _questionTypeRepository.GetByCodeAsync(type.Code);
            if (existing == null)
            {
                var questionType = new QuestionType 
                { 
                    QuestionTypeName = type.Name,
                    QuestionTypeCode = type.Code,
                    RequiresChoices = type.RequiresChoices,
                    AllowsMultipleSelection = type.AllowsMultiple,
                    IsActive = true
                };
                await _questionTypeRepository.CreateAsync(questionType);
                createdCount++;
            }
        }

        return Ok(new { message = $"{createdCount} new question types created.", totalTypes = defaultTypes.Length });
    }
}
