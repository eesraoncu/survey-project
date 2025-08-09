using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserAddress { get; set; } = string.Empty;
    public int UserAge { get; set; }
    public string AddressId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}


