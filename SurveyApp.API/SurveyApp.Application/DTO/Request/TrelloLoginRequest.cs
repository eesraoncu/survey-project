namespace SurveyApp.Application.DTO.Request;

public sealed class TrelloLoginRequest
{
    public string Token { get; set; } = string.Empty; // Trello access token
}
