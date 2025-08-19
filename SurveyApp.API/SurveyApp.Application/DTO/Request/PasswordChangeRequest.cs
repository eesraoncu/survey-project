using System.Text.Json.Serialization;

namespace SurveyApp.Application.DTO.Request;

public class PasswordChangeRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    
    public string NewPassword { get; set; } = string.Empty;
    
    public string ConfirmPassword { get; set; } = string.Empty;
}
