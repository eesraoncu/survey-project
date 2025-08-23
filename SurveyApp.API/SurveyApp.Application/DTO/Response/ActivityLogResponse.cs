namespace SurveyApp.Application.DTO.Response;

public class ActivityLogResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? ResourceId { get; set; }
    public string? ResourceType { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Kullanıcı bilgileri (opsiyonel)
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}

public class ActivityLogListResponse
{
    public List<ActivityLogResponse> Activities { get; set; } = new List<ActivityLogResponse>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class ActivityLogSummaryResponse
{
    public string ActivityType { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastActivity { get; set; }
}
