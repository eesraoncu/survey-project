using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SurveyApp.Services;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using System.Security.Claims;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventResponse>> GetEventById(int id)
    {
        try
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            return Ok(eventItem);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetEventsByUserId(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId != userId)
                return Forbid();

            var events = await _eventService.GetEventsByUserIdAsync(userId);
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetEventsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var events = await _eventService.GetEventsByDateRangeAsync(startDate, endDate);
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<EventResponse>> CreateEvent(EventRequest eventRequest)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var createdEvent = await _eventService.CreateEventAsync(eventRequest, currentUserId);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EventResponse>> UpdateEvent(int id, EventUpdateRequest eventUpdateRequest)
    {
        try
        {
            if (id != eventUpdateRequest.Id)
                return BadRequest(new { message = "ID mismatch" });

            var currentUserId = GetCurrentUserId();
            var updatedEvent = await _eventService.UpdateEventAsync(eventUpdateRequest, currentUserId);
            return Ok(updatedEvent);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var deleted = await _eventService.DeleteEventAsync(id, currentUserId);
            
            if (!deleted)
                return NotFound(new { message = "Event not found" });

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return userId;
        
        throw new InvalidOperationException("User ID not found in claims");
    }
}
