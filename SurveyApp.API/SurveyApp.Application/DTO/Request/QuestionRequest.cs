using System;
using System.Text.Json.Serialization;

namespace SurveyApp.Application.DTO.Request;

public sealed class QuestionCreateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new List<string>();
    public int SurveysId { get; set; }

    // Frontend uyumluluğu: surveyId (camelCase) ismiyle gelen alanı da destekle
    [JsonPropertyName("surveyId")] public int? SurveyId { get; set; }
    // Frontend uyumluluğu: questionText ismiyle gelen alanı da destekle
    [JsonPropertyName("questionText")] public string? QuestionText { get; set; }
}

public sealed class QuestionUpdateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new List<string>();
}

