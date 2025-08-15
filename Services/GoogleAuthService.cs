using Microsoft.Extensions.Options;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly GoogleAuthSettings _googleSettings;

    public GoogleAuthService(
        IUserRepository userRepository,
        IUserService userService,
        IOptions<GoogleAuthSettings> googleSettings)
    {
        _userRepository = userRepository;
        _userService = userService;
        _googleSettings = googleSettings.Value;
    }

    public async Task<User?> AuthenticateGoogleUserAsync(string accessToken)
    {
        try
        {
            // Google Access Token ile kullanıcı bilgilerini al
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Google API error: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var userInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(json);

            if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
            {
                Console.WriteLine("Google user info is null or email is empty");
                return null;
            }

            // Kullanıcıyı bul veya oluştur
            return await GetOrCreateUserFromGoogleAsync(userInfo.Email, userInfo.GivenName ?? "", userInfo.FamilyName ?? "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Google authentication error: {ex.Message}");
            return null;
        }
    }

    public async Task<User?> GetOrCreateUserFromGoogleAsync(string email, string name, string surname)
    {
        // Önce mevcut kullanıcıyı kontrol et
        var existingUser = await _userRepository.GetByEmailAsync(email);
        
        if (existingUser != null)
        {
            // Kullanıcı zaten varsa, rollerle birlikte döndür
            return await _userService.GetUserWithRolesAsync(existingUser.Id);
        }

        // Yeni kullanıcı oluştur
        var newUser = new User
        {
            UserName = name,
            UserSurname = surname,
            UserEmail = email,
            UserPassword = "", // Google kullanıcıları için şifre yok
            UserAge = 0, // Varsayılan yaş
            UserAddress = "",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(newUser);
        
        // Yeni kullanıcıya User rolü ekle
        await _userService.AddRoleToUserAsync(createdUser.Id, "user");
        
        // Kullanıcıyı rollerle birlikte döndür
        return await _userService.GetUserWithRolesAsync(createdUser.Id);
    }
}
