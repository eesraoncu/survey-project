using System;

namespace SurveyApp.Application.DTO.Request;

public sealed class UserRegisterRequest
{
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty; // Hashlenecek
    public int UserAge { get; set; }
    
    // Adres bilgileri - Opsiyonel, boş bırakılabilir
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? SemtBucakBelde { get; set; }
    public string? Mahalle { get; set; }
    public string? AdresDetay { get; set; } // Sokak, bina, daire vs.
    
    // RoleId artık otomatik olarak 3 (user) olacak, admin sadece manuel eklenebilir
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
    // RoleId güncellenemez - sadece sistem tarafından değiştirilebilir
}

public sealed class RoleUpdateRequest
{
    public int RoleId { get; set; } // 1: Admin, 2: Owner, 3: User
}

