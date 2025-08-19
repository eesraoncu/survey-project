using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class AnswerResponse
{
    public int Id { get; set; }
    public string AnswersText { get; set; } = string.Empty;
    public List<int> ChoiceIds { get; set; } = new List<int>();
    public List<string> SelectedChoices { get; set; } = new List<string>();
    public double? NumericValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string DisplayValue { get; set; } = string.Empty;
    public int QuestionsId { get; set; }
    public int UsersId { get; set; }
    public int SurveysId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


