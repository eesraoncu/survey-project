using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class QuestionResponse
{
    public string Id { get; set; } = string.Empty;
    public string QuestionsText { get; set; } = string.Empty;
    public string SurveysId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


