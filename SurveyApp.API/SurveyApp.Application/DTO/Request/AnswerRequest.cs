using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class AnswerCreateRequest
{
    public string AnswersText { get; set; } = string.Empty;
    public string QuestionsId { get; set; } = string.Empty;
    public string UsersId { get; set; } = string.Empty;
    public string SurveysId { get; set; } = string.Empty;
}

public sealed class AnswerUpdateRequest
{
    public string AnswersText { get; set; } = string.Empty;
}

