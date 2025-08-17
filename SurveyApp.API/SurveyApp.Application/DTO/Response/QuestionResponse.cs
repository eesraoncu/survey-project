using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class QuestionResponse
{
    public int Id { get; set; }
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new List<string>();
    public int SurveysId { get; set; }
    public DateTime CreatedAt { get; set; }
}


