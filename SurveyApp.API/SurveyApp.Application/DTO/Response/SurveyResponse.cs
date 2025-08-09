using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class SurveyResponse
{
    public string Id { get; set; } = string.Empty;
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public string UsersId { get; set; } = string.Empty;
    public string SurveyTypeId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public sealed class SurveyListItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string SurveyName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}


