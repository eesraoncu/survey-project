using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class SurveyCreateRequest
{
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public int UsersId { get; set; }
    public int SurveyTypeId { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class SurveyUpdateRequest
{
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public int SurveyTypeId { get; set; }
    public bool IsActive { get; set; } = true;
}