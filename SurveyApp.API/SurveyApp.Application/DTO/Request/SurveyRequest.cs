using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class SurveyCreateRequest
{
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public string UsersId { get; set; } = string.Empty;
    public string SurveyTypeId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class SurveyUpdateRequest
{
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public string SurveyTypeId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}