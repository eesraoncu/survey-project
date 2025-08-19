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
public sealed class SurveysController : ControllerBase
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public SurveysController(ISurveyRepository surveyRepository, IUserRepository userRepository, IUserService userService, IQuestionRepository questionRepository, IMapper mapper)
    {
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
        _userService = userService;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [HttpGet("get-all")]
    public async Task<ActionResult<List<SurveyListItemResponse>>> GetAll([FromQuery] int? usersId, [FromQuery] int? users_id, [FromQuery] int? userId)
    {
        var filterUserId = usersId ?? users_id ?? userId;
        List<Survey> list;
        if (filterUserId.HasValue && filterUserId.Value > 0)
        {
            list = await _surveyRepository.GetByUserIdAsync(filterUserId.Value);
        }
        else
        {
            list = await _surveyRepository.GetAllAsync();
        }
        var mapped = _mapper.Map<List<SurveyListItemResponse>>(list);
        return Ok(mapped);
    }

    // Kullanıcıya göre anketler
    [HttpGet("get-by-user/{userId}")]
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<List<SurveyListItemResponse>>> GetByUser(int userId)
    {
        var list = await _surveyRepository.GetByUserIdAsync(userId);
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
        
        var surveyResponse = _mapper.Map<SurveyResponse>(entity);
        
        // Anketin sorularını da getir
        var questions = await _questionRepository.GetBySurveyIdAsync(id);
        surveyResponse.Questions = _mapper.Map<List<QuestionResponse>>(questions);
        
        return Ok(surveyResponse);
    }

    // Frontend uyumluluğu: /api/Surveys/stats/{id} bekleyen çağrılar için
    [HttpGet("stats/{id}")]
    public async Task<ActionResult<SurveyResponse>> GetStats(int id)
    {
        var entity = await _surveyRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var surveyResponse = _mapper.Map<SurveyResponse>(entity);

        var questions = await _questionRepository.GetBySurveyIdAsync(id);
        surveyResponse.Questions = _mapper.Map<List<QuestionResponse>>(questions);

        return Ok(surveyResponse);
    }

    [HttpPost]
    public async Task<ActionResult<SurveyResponse>> Create([FromBody] SurveyCreateRequest request)
    {
        var entity = _mapper.Map<Survey>(request);
        var created = await _surveyRepository.CreateAsync(entity);
        
        // Survey oluşturulduğunda kullanıcıya owner rolü ekle
        var user = await _userRepository.GetByIdAsync(request.UsersId);
        if (user != null && !user.IsAdmin) // Admin değilse owner yap
        {
            await _userService.AddRoleToUserAsync(user.Id, "owner");
        }
        
        var response = _mapper.Map<SurveyResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SurveyUpdateRequest request)
    {
        var existing = await _surveyRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        
        // Sadece survey owner'ı veya admin düzenleyebilir
        // Bu kontrol için userId'ye ihtiyacımız var - JWT'den alınabilir
        // Şimdilik basit kontrol yapalım
        
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

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteSurvey(int id, [FromBody] SurveyCompletionRequest request)
    {
        var survey = await _surveyRepository.GetByIdAsync(id);
        if (survey is null) return NotFound();

        // Survey tamamlandığında kullanıcıya user rolü ekle (eğer yoksa)
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user != null && !user.IsAdmin) // Admin değilse user yap
        {
            await _userService.AddRoleToUserAsync(user.Id, "user");
        }

        survey.IsCompleted = true;
        var ok = await _surveyRepository.UpdateAsync(id, survey);
        return ok ? NoContent() : NotFound();
    }
}


