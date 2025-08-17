using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class SurveyResponse
{
    public int Id { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public int UsersId { get; set; }
    public int SurveyTypeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public List<QuestionResponse> Questions { get; set; } = new List<QuestionResponse>();
}

public sealed class SurveyListItemResponse
{
    public int Id { get; set; }
    public string SurveyName { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public int SurveyTypeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}


