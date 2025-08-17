namespace SurveyApp.Models;

public class GoogleAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public int MaxTokens { get; set; } = 1000;
    public double Temperature { get; set; } = 0.7;
}
