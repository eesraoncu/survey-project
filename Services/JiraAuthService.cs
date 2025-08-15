using Microsoft.Extensions.Options;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using System.Text;
using System.Text.Json;

namespace SurveyApp.Services;

public class JiraAuthService : IJiraAuthService
{
    private readonly JiraAuthSettings _settings;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IHttpClientFactory _httpClientFactory;

    public JiraAuthService(
        IOptions<JiraAuthSettings> settings,
        IUserRepository userRepository,
        IUserService userService,
        IHttpClientFactory httpClientFactory)
    {
        _settings = settings.Value;
        _userRepository = userRepository;
        _userService = userService;
        _httpClientFactory = httpClientFactory;
        
        Console.WriteLine("JiraAuthService initialized:");
        Console.WriteLine($"Client ID: {_settings.ClientId?.Substring(0, Math.Min(10, _settings.ClientId?.Length ?? 0))}...");
        Console.WriteLine($"Redirect URI: {_settings.RedirectUri}");
    }

    public string GetAuthorizationUrl()
    {
        var state = Guid.NewGuid().ToString();
        var scopes = "read:account read:me read:api-all:jira-align";
        
        var url = $"{_settings.AuthorizationUrl}?" +
                  $"audience=api.atlassian.com&" +
                  $"client_id={_settings.ClientId}&" +
                  $"scope={Uri.EscapeDataString(scopes)}&" +
                  $"redirect_uri={Uri.EscapeDataString(_settings.RedirectUri)}&" +
                  $"state={state}&" +
                  $"response_type=code&" +
                  $"prompt=consent";
        
        Console.WriteLine($"Jira authorization URL: {url}");
        return url;
    }

    public async Task<User?> AuthenticateJiraUserAsync(string code)
    {
        try
        {
            Console.WriteLine($"Jira authentication started with code: {code?.Substring(0, Math.Min(10, code?.Length ?? 0))}...");
            
            // Test için hard-coded token kullan (geçici çözüm)
            var testToken = "ATOA7Au-9iw2si05L2v3u7twrLpnTC6kZSqjx-rU7dz8px1dVTFZ9OXb_sTJ-atDyWlkBCBD2E10";
            
            string accessToken;
            
            // Eğer gelen code boşsa test token'ı kullan
            if (string.IsNullOrEmpty(code))
            {
                Console.WriteLine("Code is null or empty, using test token!");
                accessToken = testToken;
            }
            else
            {
                // Authorization code'u access token'a çevir
                accessToken = await ExchangeCodeForTokenAsync(code);
                if (string.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("Failed to exchange code for token");
                    return null;
                }
            }
            
            // Jira API'den kullanıcı bilgilerini al
            var userInfo = await GetJiraUserInfoAsync(accessToken);
            if (userInfo == null)
            {
                Console.WriteLine("Failed to get Jira user info");
                return null;
            }

            Console.WriteLine($"Jira user found: {userInfo.Email} - {userInfo.Name} - {userInfo.AccountId}");
            
            // Kullanıcıyı veritabanında bul veya oluştur
            var user = await GetOrCreateUserFromJiraAsync(userInfo.Email, userInfo.Name, userInfo.AccountId);
            
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Jira authentication error: {ex.Message}");
            return null;
        }
    }

    private async Task<string?> ExchangeCodeForTokenAsync(string code)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("JiraClient");
            
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl);
            tokenRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            var formData = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "authorization_code"),
                new("client_id", _settings.ClientId),
                new("client_secret", _settings.ClientSecret),
                new("code", code),
                new("redirect_uri", _settings.RedirectUri)
            };
            
            tokenRequest.Content = new FormUrlEncodedContent(formData);
            
            Console.WriteLine($"=== JIRA TOKEN EXCHANGE START ===");
            Console.WriteLine($"Token URL: {_settings.TokenUrl}");
            Console.WriteLine($"Client ID: {_settings.ClientId}");
            Console.WriteLine($"Client Secret: {_settings.ClientSecret?.Substring(0, Math.Min(10, _settings.ClientSecret?.Length ?? 0))}...");
            Console.WriteLine($"Redirect URI: {_settings.RedirectUri}");
            Console.WriteLine($"Code: {code.Substring(0, Math.Min(10, code.Length))}...");
            Console.WriteLine($"Code Length: {code.Length}");
            
            var response = await httpClient.SendAsync(tokenRequest);
            Console.WriteLine($"Token exchange response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token exchange error: {errorContent}");
                Console.WriteLine($"=== JIRA TOKEN EXCHANGE FAILED ===");
                return null;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Token exchange response: {content}");
            
            // JSON'dan access_token'ı çıkar
            using var jsonDoc = JsonDocument.Parse(content);
            var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();
            
            Console.WriteLine($"Access token: {accessToken?.Substring(0, Math.Min(10, accessToken?.Length ?? 0))}...");
            Console.WriteLine($"=== JIRA TOKEN EXCHANGE SUCCESS ===");
            
            return accessToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exchanging code for token: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine($"=== JIRA TOKEN EXCHANGE ERROR ===");
            return null;
        }
    }

    private async Task<JiraUserInfo?> GetJiraUserInfoAsync(string accessToken)
    {
        try
        {
                    // Önce accessible resources'ı al
        var resourcesRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.atlassian.com/oauth/token/accessible-resources");
        resourcesRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        resourcesRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        
        // CSP sorunlarını önlemek için ek header'lar ekle
        resourcesRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
        resourcesRequest.Headers.Add("Origin", "https://auth.atlassian.com");
            
            Console.WriteLine("Calling Jira API: https://api.atlassian.com/oauth/token/accessible-resources");
            
            var httpClient = _httpClientFactory.CreateClient("JiraClient");
        var resourcesResponse = await httpClient.SendAsync(resourcesRequest);
            Console.WriteLine($"Jira accessible resources response status: {resourcesResponse.StatusCode}");
            
            if (!resourcesResponse.IsSuccessStatusCode)
            {
                var errorContent = await resourcesResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Jira accessible resources error: {errorContent}");
                return null;
            }
            
            var resourcesContent = await resourcesResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Jira accessible resources response: {resourcesContent}");
            
                    // Şimdi kullanıcı bilgilerini al
        var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.atlassian.com/me");
        userRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        userRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        
        // CSP sorunlarını önlemek için ek header'lar ekle
        userRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
        userRequest.Headers.Add("Origin", "https://auth.atlassian.com");
            
            Console.WriteLine("Calling Jira API: https://api.atlassian.com/me");
            
            var userResponse = await httpClient.SendAsync(userRequest);
            Console.WriteLine($"Jira user info response status: {userResponse.StatusCode}");
            
            if (!userResponse.IsSuccessStatusCode)
            {
                var errorContent = await userResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Jira user info error: {errorContent}");
                return null;
            }
            
            var userContent = await userResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Jira user info response: {userContent}");
            
            var userInfo = JsonSerializer.Deserialize<JiraUserInfo>(userContent);
            return userInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Jira user info: {ex.Message}");
            return null;
        }
    }

    private async Task<User> GetOrCreateUserFromJiraAsync(string email, string fullName, string accountId)
    {
        // Email ile kullanıcıyı ara
        var existingUsers = await _userRepository.GetAllAsync();
        var existingUser = existingUsers.FirstOrDefault(u => u.UserEmail == email);
        
        if (existingUser != null)
        {
            Console.WriteLine($"Existing user found: {existingUser.UserEmail}");
            
            // Kullanıcıyı rollerle birlikte yükle
            var existingUserWithRoles = await _userService.GetUserWithRolesAsync(existingUser.Id);
            return existingUserWithRoles ?? existingUser;
        }
        
        // Yeni kullanıcı oluştur
        Console.WriteLine($"Creating user with email: {email}...");
        
        var names = fullName.Split(' ', 2);
        var firstName = names.Length > 0 ? names[0] : fullName;
        var lastName = names.Length > 1 ? names[1] : string.Empty;
        
        var newUser = new User
        {
            UserName = firstName,
            UserSurname = lastName,
            UserEmail = email,
            UserPassword = Guid.NewGuid().ToString(), // Rastgele şifre (Jira ile giriş yapacak)
            UserAge = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        var createdUser = await _userRepository.CreateAsync(newUser);
        
        // Yeni kullanıcıya "user" rolü ekle
        await _userService.AddRoleToUserAsync(createdUser.Id, "user");
        
        // Kullanıcıyı rollerle birlikte yükle
        var userWithRoles = await _userService.GetUserWithRolesAsync(createdUser.Id);
        return userWithRoles ?? createdUser;
    }
}
