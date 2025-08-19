using System;
using System.Text.Json.Serialization;

namespace SurveyApp.Application.DTO.Request;

public sealed class QuestionTypeCreateRequest
{
    public string QuestionTypeName { get; set; } = string.Empty;
    public string QuestionTypeCode { get; set; } = string.Empty;
    public bool RequiresChoices { get; set; } = false;
    public bool AllowsMultipleSelection { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

public sealed class QuestionTypeUpdateRequest
{
    public string QuestionTypeName { get; set; } = string.Empty;
    public string QuestionTypeCode { get; set; } = string.Empty;
    public bool RequiresChoices { get; set; } = false;
    public bool AllowsMultipleSelection { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
