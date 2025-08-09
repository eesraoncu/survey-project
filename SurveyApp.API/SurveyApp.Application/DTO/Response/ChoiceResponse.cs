using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class ChoiceResponse
{
    public string Id { get; set; } = string.Empty;
    public string ChoiceText { get; set; } = string.Empty;
    public string QuestionsId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


