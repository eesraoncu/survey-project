using System;

namespace SurveyApp.Application.DTO.Response;

public sealed class UserResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserAddress { get; set; } = string.Empty;
    public int UserAge { get; set; }
    public int AddressId { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}


