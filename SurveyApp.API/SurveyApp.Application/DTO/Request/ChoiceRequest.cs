using System;
using System.Text.Json.Serialization;

namespace SurveyApp.Application.DTO.Request;

public sealed class ChoiceCreateRequest
{
    public string ChoiceText { get; set; } = string.Empty;
    public int QuestionsId { get; set; }

    // Frontend uyumluluğu
    [JsonPropertyName("questionId")] public int? QuestionId { get; set; }
}

public sealed class ChoiceUpdateRequest
{
    public string ChoiceText { get; set; } = string.Empty;
    public int QuestionsId { get; set; }
}

public sealed class BulkChoiceCreateRequest
{
    public int QuestionsId { get; set; }
    public List<string> ChoiceTexts { get; set; } = new List<string>();

    // Frontend uyumluluğu
    [JsonPropertyName("questionId")] public int? QuestionId { get; set; }
}