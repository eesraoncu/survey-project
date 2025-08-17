using AutoMapper;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public EventService(IEventRepository eventRepository, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EventResponse>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EventResponse>>(events);
    }

    public async Task<EventResponse?> GetEventByIdAsync(int id)
    {
        var eventItem = await _eventRepository.GetByIdAsync(id);
        return _mapper.Map<EventResponse>(eventItem);
    }

    public async Task<IEnumerable<EventResponse>> GetEventsByUserIdAsync(int userId)
    {
        var events = await _eventRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<EventResponse>>(events);
    }

    public async Task<IEnumerable<EventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var events = await _eventRepository.GetByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<EventResponse>>(events);
    }

    public async Task<EventResponse> CreateEventAsync(EventRequest eventRequest, int userId)
    {
        var eventItem = _mapper.Map<Event>(eventRequest);
        eventItem.UserId = userId;
        eventItem.CreatedAt = DateTime.UtcNow;
        eventItem.UpdatedAt = DateTime.UtcNow;

        var createdEvent = await _eventRepository.CreateAsync(eventItem);
        return _mapper.Map<EventResponse>(createdEvent);
    }

    public async Task<EventResponse> UpdateEventAsync(EventUpdateRequest eventUpdateRequest, int userId)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(eventUpdateRequest.Id);
        if (existingEvent == null)
            throw new InvalidOperationException("Event not found");

        if (existingEvent.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own events");

        var eventItem = _mapper.Map<Event>(eventUpdateRequest);
        eventItem.UserId = userId;
        eventItem.CreatedAt = existingEvent.CreatedAt;
        eventItem.UpdatedAt = DateTime.UtcNow;

        var updatedEvent = await _eventRepository.UpdateAsync(eventItem);
        return _mapper.Map<EventResponse>(updatedEvent);
    }

    public async Task<bool> DeleteEventAsync(int id, int userId)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(id);
        if (existingEvent == null)
            return false;

        if (existingEvent.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own events");

        return await _eventRepository.DeleteAsync(id);
    }

    public async Task<bool> EventExistsAsync(int id)
    {
        return await _eventRepository.ExistsAsync(id);
    }
}
