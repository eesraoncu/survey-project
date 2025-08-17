using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class QuestionCreateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new List<string>();
    public int SurveysId { get; set; }
}

public sealed class QuestionUpdateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new List<string>();
}

