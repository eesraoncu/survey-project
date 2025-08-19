using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class ChoiceResponse
{
    public int Id { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public int QuestionsId { get; set; }
    public DateTime CreatedAt { get; set; }
}