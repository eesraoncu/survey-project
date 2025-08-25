using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Services;
using AutoMapper;
using SurveyApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public sealed class ActivityLogController : ControllerBase
{
    private readonly IActivityLogService _activityLogService;
    private readonly IMapper _mapper;

    public ActivityLogController(IActivityLogService activityLogService, IMapper mapper)
    {
        _activityLogService = activityLogService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ActivityLogListResponse>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var activities = await _activityLogService.GetRecentActivitiesAsync(pageSize);
            var totalCount = await _activityLogService.GetActivityCountAsync();
            
            var response = new ActivityLogListResponse
            {
                Activities = _mapper.Map<List<ActivityLogResponse>>(activities),
                TotalCount = (int)totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Aktivite logları alınırken hata oluştu", error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ActivityLogResponse>>> GetByUserId(int userId, [FromQuery] int limit = 50)
    {
        try
        {
            var activities = await _activityLogService.GetUserActivitiesAsync(userId, limit);
            var response = _mapper.Map<List<ActivityLogResponse>>(activities);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Kullanıcı aktiviteleri alınırken hata oluştu", error = ex.Message });
        }
    }

    [HttpGet("type/{activityType}")]
    public async Task<ActionResult<List<ActivityLogResponse>>> GetByActivityType(string activityType, [FromQuery] int limit = 50)
    {
        try
        {
            var activities = await _activityLogService.GetActivitiesByTypeAsync(activityType, limit);
            var response = _mapper.Map<List<ActivityLogResponse>>(activities);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Aktivite tipi logları alınırken hata oluştu", error = ex.Message });
        }
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<List<ActivityLogResponse>>> GetByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            var activities = await _activityLogService.GetActivitiesByDateRangeAsync(startDate, endDate);
            var response = _mapper.Map<List<ActivityLogResponse>>(activities);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Tarih aralığı logları alınırken hata oluştu", error = ex.Message });
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<long>> GetCount()
    {
        try
        {
            var count = await _activityLogService.GetActivityCountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Log sayısı alınırken hata oluştu", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _activityLogService.DeleteActivityAsync(id);
            if (result)
            {
                return Ok(new { message = "Aktivite logu başarıyla silindi" });
            }
            return NotFound(new { message = "Aktivite logu bulunamadı" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Aktivite logu silinirken hata oluştu", error = ex.Message });
        }
    }
}
