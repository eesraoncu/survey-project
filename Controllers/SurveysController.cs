using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SurveysController : ControllerBase
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IMapper _mapper;

    public SurveysController(ISurveyRepository surveyRepository, IMapper mapper)
    {
        _surveyRepository = surveyRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<SurveyListItemResponse>>> GetAll()
    {
        var list = await _surveyRepository.GetAllAsync();
        return Ok(_mapper.Map<List<SurveyListItemResponse>>(list));
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<SurveyListItemResponse>>> GetActive()
    {
        var list = await _surveyRepository.GetActiveSurveysAsync();
        return Ok(_mapper.Map<List<SurveyListItemResponse>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SurveyResponse>> GetById(int id)
    {
        var entity = await _surveyRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<SurveyResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<SurveyResponse>> Create([FromBody] SurveyCreateRequest request)
    {
        var entity = _mapper.Map<Survey>(request);
        var created = await _surveyRepository.CreateAsync(entity);
        var response = _mapper.Map<SurveyResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SurveyUpdateRequest request)
    {
        var existing = await _surveyRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        _mapper.Map(request, existing);
        var ok = await _surveyRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _surveyRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}


