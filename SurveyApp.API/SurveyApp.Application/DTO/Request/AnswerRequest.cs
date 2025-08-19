using System;
using System.Text.Json.Serialization;

namespace SurveyApp.Application.DTO.Request;

public sealed class AnswerCreateRequest
{
    public string AnswersText { get; set; } = string.Empty;
    public List<int> ChoiceIds { get; set; } = new List<int>();
    public List<string> SelectedChoices { get; set; } = new List<string>();
    public double? NumericValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
    public int QuestionsId { get; set; }
    public int UsersId { get; set; }
    public int SurveysId { get; set; }

    // Frontend uyumluluğu
    [JsonPropertyName("questionId")] public int? QuestionId { get; set; }
    [JsonPropertyName("surveyId")] public int? SurveyId { get; set; }
    [JsonPropertyName("userId")] public int? UserId { get; set; }
}

public sealed class AnswerUpdateRequest
{
    public string AnswersText { get; set; } = string.Empty;
    public List<int> ChoiceIds { get; set; } = new List<int>();
    public List<string> SelectedChoices { get; set; } = new List<string>();
    public double? NumericValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
}

public sealed class BulkAnswerCreateRequest
{
    public int UsersId { get; set; }
    public int SurveysId { get; set; }
    public List<AnswerCreateRequest> Answers { get; set; } = new List<AnswerCreateRequest>();

    // Frontend uyumluluğu
    [JsonPropertyName("surveyId")] public int? SurveyId { get; set; }
    [JsonPropertyName("userId")] public int? UserId { get; set; }
}

