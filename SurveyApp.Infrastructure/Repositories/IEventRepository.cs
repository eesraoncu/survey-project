using SurveyApp.Models;

namespace SurveyApp.Infrastructure.Repositories;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int id);
    Task<IEnumerable<Event>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Event> CreateAsync(Event eventItem);
    Task<Event> UpdateAsync(Event eventItem);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
