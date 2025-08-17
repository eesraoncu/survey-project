namespace SurveyApp.Application.DTO.Request;

public class EventRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAllDay { get; set; } = false;
    public string? Location { get; set; }
    public string Color { get; set; } = "#3788d8";
}

public class EventUpdateRequest : EventRequest
{
    public int Id { get; set; }
}
