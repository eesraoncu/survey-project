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
    private readonly IActivityLogService _activityLogService;

    public SurveysController(ISurveyRepository surveyRepository, IUserRepository userRepository, IUserService userService, IQuestionRepository questionRepository, IMapper mapper, IActivityLogService activityLogService)
    {
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
        _userService = userService;
        _questionRepository = questionRepository;
        _mapper = mapper;
        _activityLogService = activityLogService;
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
        try
        {
            var entity = _mapper.Map<Survey>(request);
            var created = await _surveyRepository.CreateAsync(entity);
            
            // Survey oluşturulduğunda kullanıcıya owner rolü ekle
            var user = await _userRepository.GetByIdAsync(request.UsersId);
            if (user != null && !user.IsAdmin) // Admin değilse owner yap
            {
                await _userService.AddRoleToUserAsync(user.Id, "owner");
            }
            
            // Anket oluşturma logu
            await _activityLogService.LogActivityAsync(
                userId: request.UsersId,
                activityType: "survey_created",
                description: $"Yeni anket oluşturuldu: {request.SurveyName}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString(),
                resourceId: created.Id,
                resourceType: "survey",
                additionalData: new Dictionary<string, object>
                {
                    ["survey_name"] = request.SurveyName,
                    ["survey_type_id"] = request.SurveyTypeId
                },
                isSuccessful: true
            );
            
            var response = _mapper.Map<SurveyResponse>(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            // Hata logu
            await _activityLogService.LogActivityAsync(
                userId: request.UsersId,
                activityType: "survey_creation_failed",
                description: $"Anket oluşturma hatası: {request.SurveyName}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString(),
                isSuccessful: false,
                errorMessage: ex.Message
            );
            
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SurveyUpdateRequest request)
    {
        try
        {
            var existing = await _surveyRepository.GetByIdAsync(id);
            if (existing is null) 
            {
                // Anket bulunamadı logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UsersId,
                    activityType: "survey_update_failed",
                    description: $"Anket güncelleme hatası: Anket bulunamadı (ID: {id})",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    isSuccessful: false,
                    errorMessage: "Anket bulunamadı"
                );
                return NotFound();
            }
            
            // Eski değerleri sakla (log için)
            var oldSurveyName = existing.SurveyName;
            var oldSurveyDescription = existing.SurveyDescription;
            
            // Sadece survey owner'ı veya admin düzenleyebilir
            // Bu kontrol için userId'ye ihtiyacımız var - JWT'den alınabilir
            // Şimdilik basit kontrol yapalım
            
            _mapper.Map(request, existing);
            var ok = await _surveyRepository.UpdateAsync(id, existing);
            
            if (ok)
            {
                // Başarılı güncelleme logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UsersId,
                    activityType: "survey_updated",
                    description: $"Anket güncellendi: {oldSurveyName} → {request.SurveyName}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    additionalData: new Dictionary<string, object>
                    {
                        ["old_survey_name"] = oldSurveyName,
                        ["new_survey_name"] = request.SurveyName,
                        ["old_survey_description"] = oldSurveyDescription,
                        ["new_survey_description"] = request.SurveyDescription,
                        ["survey_type_id"] = request.SurveyTypeId
                    },
                    isSuccessful: true
                );
                return NoContent();
            }
            else
            {
                // Güncelleme başarısız logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UsersId,
                    activityType: "survey_update_failed",
                    description: $"Anket güncelleme hatası: {request.SurveyName}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    isSuccessful: false,
                    errorMessage: "Veritabanı güncelleme hatası"
                );
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            // Hata logu
            await _activityLogService.LogActivityAsync(
                userId: request.UsersId,
                activityType: "survey_update_failed",
                description: $"Anket güncelleme hatası: {request.SurveyName}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString(),
                resourceId: id,
                resourceType: "survey",
                isSuccessful: false,
                errorMessage: ex.Message
            );
            
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SurveyDeleteResponse>> Delete(int id, [FromBody] SurveyDeleteRequest request)
    {
        try
        {
            var existing = await _surveyRepository.GetByIdAsync(id);
            if (existing is null) 
            {
                // Anket bulunamadı logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UsersId,
                    activityType: "survey_delete_failed",
                    description: $"Anket silme hatası: Anket bulunamadı (ID: {id})",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    additionalData: new Dictionary<string, object>
                    {
                        ["deleted_by_user_id"] = request.UsersId,
                        ["error_type"] = "survey_not_found"
                    },
                    isSuccessful: false,
                    errorMessage: "Anket bulunamadı"
                );
                return NotFound(new SurveyDeleteResponse
                {
                    Success = false,
                    Message = "Anket bulunamadı",
                    DeletedSurveyId = id,
                    DeletedByUserId = request.UsersId
                });
            }
            
            // Anket bilgilerini sakla (log için)
            var surveyName = existing.SurveyName;
            var surveyDescription = existing.SurveyDescription;
            var surveyOwnerId = existing.UsersId;
            
            // Silen kullanıcının bilgilerini al
            var deletingUser = await _userRepository.GetByIdAsync(request.UsersId);
            var deletingUserEmail = deletingUser?.UserEmail ?? "Bilinmeyen Kullanıcı";
            
            // Sadece survey owner'ı veya admin silebilir
            // Bu kontrol için userId'ye ihtiyacımız var - JWT'den alınabilir
            // Şimdilik basit kontrol yapalım
            
            var ok = await _surveyRepository.DeleteAsync(id);
            
            if (ok)
            {
                // Başarılı silme logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UsersId,
                    activityType: "survey_deleted",
                    description: $"Anket silindi: {surveyName}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    additionalData: new Dictionary<string, object>
                    {
                        ["survey_name"] = surveyName,
                        ["survey_description"] = surveyDescription,
                        ["survey_owner_id"] = surveyOwnerId,
                        ["deleted_by_user_id"] = request.UsersId,
                        ["deleted_by_user_email"] = deletingUserEmail,
                        ["delete_reason"] = request.Reason ?? "Belirtilmedi"
                    },
                    isSuccessful: true
                );
                
                // Başarılı silme response'u
                return Ok(new SurveyDeleteResponse
                {
                    Success = true,
                    Message = $"Anket başarıyla silindi: {surveyName}",
                    DeletedSurveyId = id,
                    DeletedSurveyName = surveyName,
                    DeletedByUserId = request.UsersId,
                    DeletedByUserEmail = deletingUserEmail,
                    DeleteReason = request.Reason ?? "Belirtilmedi",
                    DeletedAt = DateTime.UtcNow
                });
            }
            else
            {
                // Silme başarısız logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UsersId,
                    activityType: "survey_delete_failed",
                    description: $"Anket silme hatası: {surveyName}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    additionalData: new Dictionary<string, object>
                    {
                        ["survey_name"] = surveyName,
                        ["survey_description"] = surveyDescription,
                        ["survey_owner_id"] = surveyOwnerId,
                        ["deleted_by_user_id"] = request.UsersId,
                        ["deleted_by_user_email"] = deletingUserEmail,
                        ["delete_reason"] = request.Reason ?? "Belirtilmedi"
                    },
                    isSuccessful: false,
                    errorMessage: "Veritabanı silme hatası"
                );
                
                return NotFound(new SurveyDeleteResponse
                {
                    Success = false,
                    Message = "Anket silme işlemi başarısız",
                    DeletedSurveyId = id,
                    DeletedSurveyName = surveyName,
                    DeletedByUserId = request.UsersId,
                    DeletedByUserEmail = deletingUserEmail
                });
            }
        }
        catch (Exception ex)
        {
            // Hata logu
            await _activityLogService.LogActivityAsync(
                userId: request.UsersId,
                activityType: "survey_delete_failed",
                description: $"Anket silme hatası (ID: {id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString(),
                resourceId: id,
                resourceType: "survey",
                additionalData: new Dictionary<string, object>
                {
                    ["deleted_by_user_id"] = request.UsersId,
                    ["error_type"] = "exception"
                },
                isSuccessful: false,
                errorMessage: ex.Message
            );
            
            return BadRequest(new SurveyDeleteResponse
            {
                Success = false,
                Message = $"Anket silme hatası: {ex.Message}",
                DeletedSurveyId = id,
                DeletedByUserId = request.UsersId
            });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteSurvey(int id, [FromBody] SurveyCompletionRequest request)
    {
        try
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey is null) 
            {
                // Anket bulunamadı logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UserId,
                    activityType: "survey_completion_failed",
                    description: $"Anket tamamlama hatası: Anket bulunamadı (ID: {id})",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    isSuccessful: false,
                    errorMessage: "Anket bulunamadı"
                );
                return NotFound();
            }

            // Survey tamamlandığında kullanıcıya user rolü ekle (eğer yoksa)
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user != null && !user.IsAdmin) // Admin değilse user yap
            {
                await _userService.AddRoleToUserAsync(user.Id, "user");
            }

            survey.IsCompleted = true;
            var ok = await _surveyRepository.UpdateAsync(id, survey);
            
            if (ok)
            {
                // Başarılı tamamlama logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UserId,
                    activityType: "survey_completed",
                    description: $"Anket tamamlandı: {survey.SurveyName}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    additionalData: new Dictionary<string, object>
                    {
                        ["survey_name"] = survey.SurveyName,
                        ["survey_owner_id"] = survey.UsersId,
                        ["completion_date"] = DateTime.UtcNow
                    },
                    isSuccessful: true
                );
                return NoContent();
            }
            else
            {
                // Tamamlama başarısız logu
                await _activityLogService.LogActivityAsync(
                    userId: request.UserId,
                    activityType: "survey_completion_failed",
                    description: $"Anket tamamlama hatası: {survey.SurveyName}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    resourceId: id,
                    resourceType: "survey",
                    isSuccessful: false,
                    errorMessage: "Veritabanı güncelleme hatası"
                );
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            // Hata logu
            await _activityLogService.LogActivityAsync(
                userId: request.UserId,
                activityType: "survey_completion_failed",
                description: $"Anket tamamlama hatası (ID: {id})",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString(),
                resourceId: id,
                resourceType: "survey",
                isSuccessful: false,
                errorMessage: ex.Message
            );
            
            throw;
        }
    }
}


