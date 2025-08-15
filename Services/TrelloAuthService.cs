using SurveyApp.Models;
using SurveyApp.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;

namespace SurveyApp.Services;

public class TrelloAuthService : ITrelloAuthService
{
    private readonly TrelloAuthSettings _trelloSettings;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public TrelloAuthService(
        IOptions<TrelloAuthSettings> trelloSettings,
        IUserRepository userRepository,
        IUserService userService)
    {
        _trelloSettings = trelloSettings.Value;
        _userRepository = userRepository;
        _userService = userService;
        
        // Configuration kontrolü
        Console.WriteLine($"TrelloAuthService initialized:");
        Console.WriteLine($"API Key: {_trelloSettings.ApiKey?.Substring(0, Math.Min(10, _trelloSettings.ApiKey?.Length ?? 0))}...");
        Console.WriteLine($"API Secret: {_trelloSettings.ApiSecret?.Substring(0, Math.Min(10, _trelloSettings.ApiSecret?.Length ?? 0))}...");
        Console.WriteLine($"Redirect URI: {_trelloSettings.RedirectUri}");
    }

    public string GetAuthorizationUrl()
    {
        var scope = "read,write";
        var responseType = "token";
        var returnUrl = HttpUtility.UrlEncode(_trelloSettings.RedirectUri);
        var expiration = "never"; // Token süresiz olsun
        
        return $"https://trello.com/1/authorize?key={_trelloSettings.ApiKey}&name=SurveyApp&scope={scope}&response_type={responseType}&return_url={returnUrl}&expiration={expiration}";
    }

    public async Task<User?> AuthenticateTrelloUserAsync(string token)
    {
        try
        {
            Console.WriteLine($"Trello authentication started with token: {token?.Substring(0, Math.Min(10, token?.Length ?? 0))}...");
            
            // Get user info from Trello using the provided token
            var userInfo = await GetTrelloUserInfoAsync(token);
            if (userInfo == null)
            {
                Console.WriteLine("Trello user info is null");
                return null;
            }
            
            // Email boş olsa bile devam et (username ile email oluşturacağız)
            Console.WriteLine($"Trello user found: {userInfo.Email ?? "NO_EMAIL"} - {userInfo.FullName} - {userInfo.Username}");
            return await GetOrCreateUserFromTrelloAsync(userInfo.Email, userInfo.FullName, userInfo.Username);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Trello authentication error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<User?> GetOrCreateUserFromTrelloAsync(string email, string fullName, string username)
    {
        try
        {
            // Email boşsa username ile oluştur
            var userEmail = !string.IsNullOrEmpty(email) ? email : $"{username}@trello.com";
            
            Console.WriteLine($"Creating user with email: {userEmail}, fullName: {fullName}, username: {username}");
            
            // Check if user already exists by email or username
            var existingUsers = await _userRepository.GetAllAsync();
            var existingUser = existingUsers.FirstOrDefault(u => 
                u.UserEmail == userEmail || 
                u.UserEmail == $"{username}@trello.com");

            if (existingUser != null)
            {
                Console.WriteLine($"Existing user found: {existingUser.UserEmail}");
                // User exists, return with roles
                return await _userService.GetUserWithRolesAsync(existingUser.Id);
            }

            // Create new user
            var newUser = new User
            {
                UserName = fullName.Split(' ').FirstOrDefault() ?? username,
                UserSurname = fullName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                UserEmail = userEmail,
                UserPassword = "", // Trello users don't have password
                UserAge = 0,
                AddressId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            Console.WriteLine($"Creating new user: {newUser.UserName} {newUser.UserSurname} - {newUser.UserEmail}");
            var createdUser = await _userRepository.CreateAsync(newUser);

            // Add default user role
            await _userService.AddRoleToUserAsync(createdUser.Id, "user");

            // Return user with roles
            return await _userService.GetUserWithRolesAsync(createdUser.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user from Trello: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    // Bu metod artık gerekli değil çünkü Trello doğrudan token döndürüyor
    // private async Task<string?> ExchangeCodeForTokenAsync(string code) metodunu kaldırdık

    private async Task<TrelloUserInfo?> GetTrelloUserInfoAsync(string token)
    {
        try
        {
            using var httpClient = new HttpClient();
            
            var url = $"https://api.trello.com/1/members/me?key={_trelloSettings.ApiKey}&token={token}";
            Console.WriteLine($"Calling Trello API: {url}");
            
            // Get user info using the token
            var response = await httpClient.GetAsync(url);
            
            Console.WriteLine($"Trello API response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Trello user info request failed: {response.StatusCode}");
                Console.WriteLine($"Error content: {errorContent}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Trello API response: {json}");
            
            return System.Text.Json.JsonSerializer.Deserialize<TrelloUserInfo>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Trello user info: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }
}
