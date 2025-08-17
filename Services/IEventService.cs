using SurveyApp.Models;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Services;

public interface IEventService
{
    Task<IEnumerable<EventResponse>> GetAllEventsAsync();
    Task<EventResponse?> GetEventByIdAsync(int id);
    Task<IEnumerable<EventResponse>> GetEventsByUserIdAsync(int userId);
    Task<IEnumerable<EventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<EventResponse> CreateEventAsync(EventRequest eventRequest, int userId);
    Task<EventResponse> UpdateEventAsync(EventUpdateRequest eventUpdateRequest, int userId);
    Task<bool> DeleteEventAsync(int id, int userId);
    Task<bool> EventExistsAsync(int id);
}
