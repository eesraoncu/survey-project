using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserResponse? User { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }
}
