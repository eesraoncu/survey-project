namespace SurveyApp.Application.DTO.Response;

public class SurveyDeleteResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int DeletedSurveyId { get; set; }
    public string DeletedSurveyName { get; set; } = string.Empty;
    public int DeletedByUserId { get; set; }
    public string DeletedByUserEmail { get; set; } = string.Empty;
    public string DeleteReason { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
}
