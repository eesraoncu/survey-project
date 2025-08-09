using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class QuestionCreateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
    public string SurveysId { get; set; } = string.Empty;
}

public sealed class QuestionUpdateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
}

