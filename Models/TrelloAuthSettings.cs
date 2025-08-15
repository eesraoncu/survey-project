namespace SurveyApp.Models;

public class TrelloAuthSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
