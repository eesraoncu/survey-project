using System;
using SurveyApp.Models;

namespace SurveyApp.Application.DTO.Response;

public sealed class QuestionResponse
{
    public int Id { get; set; }
    public string QuestionsText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public int QuestionTypeId { get; set; }
    public string TypeDisplayName { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public bool RequiresChoices { get; set; }
    public bool AllowsMultipleSelection { get; set; }
    public List<string> Choices { get; set; } = new List<string>(); // Backward compatibility
    public List<ChoiceResponse> ChoiceOptions { get; set; } = new List<ChoiceResponse>();
    public QuestionTypeResponse? QuestionTypeDetails { get; set; } // Question type detayları
    public int SurveysId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


