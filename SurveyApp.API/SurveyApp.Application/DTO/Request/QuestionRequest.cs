using System;
using System.Text.Json.Serialization;
using SurveyApp.Models;

namespace SurveyApp.Application.DTO.Request;

public sealed class QuestionCreateRequest
{
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty; // Question type code
    public int QuestionTypeId { get; set; } = 0; // Foreign key to QuestionType collection
    public bool IsRequired { get; set; } = false;
    public int Order { get; set; } = 0;
    public List<string> Choices { get; set; } = new List<string>(); // Backward compatibility
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
    public int QuestionTypeId { get; set; } = 0;
    public bool IsRequired { get; set; } = false;
    public int Order { get; set; } = 0;
    public List<string> Choices { get; set; } = new List<string>();
}

