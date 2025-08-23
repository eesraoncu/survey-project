using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogRepository _activityLogRepository;

    public ActivityLogService(IActivityLogRepository activityLogRepository)
    {
        _activityLogRepository = activityLogRepository;
    }

    public async Task<ActivityLog> LogActivityAsync(int userId, string activityType, string description, 
        string? ipAddress = null, string? userAgent = null, int? resourceId = null, 
        string? resourceType = null, Dictionary<string, object>? additionalData = null, 
        bool isSuccessful = true, string? errorMessage = null)
    {
        var activityLog = new ActivityLog
        {
            UserId = userId,
            ActivityType = activityType,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ResourceId = resourceId,
            ResourceType = resourceType,
            AdditionalData = additionalData,
            IsSuccessful = isSuccessful,
            ErrorMessage = errorMessage,
            CreatedAt = DateTime.UtcNow
        };

        return await _activityLogRepository.CreateAsync(activityLog);
    }

    public async Task<List<ActivityLog>> GetUserActivitiesAsync(int userId, int limit = 50)
    {
        var activities = await _activityLogRepository.GetByUserIdAsync(userId);
        return activities.Take(limit).ToList();
    }

    public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int limit = 50)
    {
        return await _activityLogRepository.GetRecentAsync(limit);
    }

    public async Task<List<ActivityLog>> GetActivitiesByTypeAsync(string activityType, int limit = 50)
    {
        var activities = await _activityLogRepository.GetByActivityTypeAsync(activityType);
        return activities.Take(limit).ToList();
    }

    public async Task<List<ActivityLog>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _activityLogRepository.GetByDateRangeAsync(startDate, endDate);
    }

    public async Task<long> GetActivityCountAsync()
    {
        return await _activityLogRepository.GetCountAsync();
    }

    public async Task<bool> DeleteActivityAsync(int activityId)
    {
        return await _activityLogRepository.DeleteAsync(activityId);
    }
}
