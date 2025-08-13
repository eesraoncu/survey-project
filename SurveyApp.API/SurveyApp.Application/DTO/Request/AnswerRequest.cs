using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class AnswerCreateRequest
{
    public string AnswersText { get; set; } = string.Empty;
    public int QuestionsId { get; set; }
    public int UsersId { get; set; }
    public int SurveysId { get; set; }
}

public sealed class AnswerUpdateRequest
{
    public string AnswersText { get; set; } = string.Empty;
}

