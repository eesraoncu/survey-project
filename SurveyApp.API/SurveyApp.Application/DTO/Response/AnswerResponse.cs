using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class AnswerResponse
{
    public int Id { get; set; }
    public string AnswersText { get; set; } = string.Empty;
    public int QuestionsId { get; set; }
    public int UsersId { get; set; }
    public int SurveysId { get; set; }
    public DateTime CreatedAt { get; set; }
}


