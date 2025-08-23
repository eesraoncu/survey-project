using SurveyApp.Models;

namespace SurveyApp.Services;

public interface IActivityLogService
{
    Task<ActivityLog> LogActivityAsync(int userId, string activityType, string description, 
        string? ipAddress = null, string? userAgent = null, int? resourceId = null, 
        string? resourceType = null, Dictionary<string, object>? additionalData = null, 
        bool isSuccessful = true, string? errorMessage = null);
    
    Task<List<ActivityLog>> GetUserActivitiesAsync(int userId, int limit = 50);
    Task<List<ActivityLog>> GetRecentActivitiesAsync(int limit = 50);
    Task<List<ActivityLog>> GetActivitiesByTypeAsync(string activityType, int limit = 50);
    Task<List<ActivityLog>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<long> GetActivityCountAsync();
    Task<bool> DeleteActivityAsync(int activityId);
}
