namespace SurveyApp.Application.DTO.Response;

public sealed class AdminResponse
{
    public int Id { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public string AdminSurname { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<string> Roles { get; set; } = new() { "admin" };
}

public sealed class AdminAuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AdminResponse? Admin { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }
}


