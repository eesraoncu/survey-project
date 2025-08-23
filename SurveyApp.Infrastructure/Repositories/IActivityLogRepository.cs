using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IActivityLogRepository
{
    Task<List<ActivityLog>> GetAllAsync();
    Task<ActivityLog?> GetByIdAsync(int id);
    Task<List<ActivityLog>> GetByUserIdAsync(int userId);
    Task<List<ActivityLog>> GetByActivityTypeAsync(string activityType);
    Task<List<ActivityLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<ActivityLog>> GetByResourceTypeAsync(string resourceType, int? resourceId = null);
    Task<ActivityLog> CreateAsync(ActivityLog activityLog);
    Task<bool> DeleteAsync(int id);
    Task<long> GetCountAsync();
    Task<List<ActivityLog>> GetRecentAsync(int limit = 50);
}
