namespace SurveyApp.Application.DTO.Response;

public class EventResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public string? Location { get; set; }
    public string Color { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class EventListResponse
{
    public List<EventResponse> Events { get; set; } = new List<EventResponse>();
    public int TotalCount { get; set; }
}
