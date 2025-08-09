using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class AnswerResponse
{
    public string Id { get; set; } = string.Empty;
    public string AnswersText { get; set; } = string.Empty;
    public string QuestionsId { get; set; } = string.Empty;
    public string UsersId { get; set; } = string.Empty;
    public string SurveysId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


