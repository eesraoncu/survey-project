using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class ChoiceCreateRequest
{
    public string ChoiceText { get; set; } = string.Empty;
    public string QuestionsId { get; set; } = string.Empty;
}

public sealed class ChoiceUpdateRequest
{
    public string ChoiceText { get; set; } = string.Empty;
}

