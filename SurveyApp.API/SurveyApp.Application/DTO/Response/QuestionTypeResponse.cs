using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class QuestionTypeResponse
{
    public int Id { get; set; }
    public string QuestionTypeName { get; set; } = string.Empty;
    public string QuestionTypeCode { get; set; } = string.Empty;
    public bool RequiresChoices { get; set; }
    public bool AllowsMultipleSelection { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
