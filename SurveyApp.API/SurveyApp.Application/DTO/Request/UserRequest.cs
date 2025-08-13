using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class UserRegisterRequest
{
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty; // Hashlenecek
    public int AddressId { get; set; }
    public int UserAge { get; set; }
    public int RoleId { get; set; }
}

public sealed class UserLoginRequest
{
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}

public sealed class UserUpdateRequest
{
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserAddress { get; set; } = string.Empty;
    public int UserAge { get; set; }
    public int RoleId { get; set; }
}

