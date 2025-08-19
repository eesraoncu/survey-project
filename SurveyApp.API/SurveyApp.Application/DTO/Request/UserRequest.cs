using System;
using System.Text.Json.Serialization;

namespace SurveyApp.Application.DTO.Request;

public sealed class UserRegisterRequest
{
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty; // Hashlenecek
    public int UserAge { get; set; }
    
    // Adres bilgileri - Frontend'den gelen format
    [JsonPropertyName("cityName")]
    public string? CityName { get; set; }
    
    [JsonPropertyName("districtName")] 
    public string? DistrictName { get; set; }
    
    [JsonPropertyName("districtTownshipTownName")]
    public string? DistrictTownshipTownName { get; set; }
    
    [JsonPropertyName("neighbourhoodName")]
    public string? NeighbourhoodName { get; set; }
    
    [JsonPropertyName("addressDetails")]
    public string? AddressDetails { get; set; }
    
    // Backward compatibility için eski property'ler
    public string? Il => CityName;
    public string? Ilce => DistrictName;
    public string? SemtBucakBelde => DistrictTownshipTownName;
    public string? Mahalle => NeighbourhoodName;
    public string? AdresDetay => AddressDetails;
}

public sealed class UserLoginRequest
{
    public string UserEmail { get; set; } = string.Empty;
    
    public string UserPassword { get; set; } = string.Empty;
    
    public string? EncryptedPassword { get; set; }
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

