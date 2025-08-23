using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using SurveyApp.Services;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IAdresService _adresService;
    private readonly IPasswordService _passwordService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ITrelloAuthService _trelloAuthService;
    private readonly IJiraAuthService _jiraAuthService;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IRsaCryptoService _rsaCryptoService;
    private readonly IActivityLogService _activityLogService;

    public AuthController(
        IUserRepository userRepository,
        IUserService userService,
        IAdresService adresService, 
        IPasswordService passwordService,
        IGoogleAuthService googleAuthService,
        ITrelloAuthService trelloAuthService,
        IJiraAuthService jiraAuthService,
        IJwtService jwtService,
        IMapper mapper,
        IRsaCryptoService rsaCryptoService,
        IActivityLogService activityLogService)
    {
        _userRepository = userRepository;
        _userService = userService;
        _adresService = adresService;
        _passwordService = passwordService;
        _googleAuthService = googleAuthService;
        _trelloAuthService = trelloAuthService;
        _jiraAuthService = jiraAuthService;
        _jwtService = jwtService;
        _mapper = mapper;
        _rsaCryptoService = rsaCryptoService;
        _activityLogService = activityLogService;
    }

    [HttpGet("login-public-key")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult<object> GetLoginPublicKey()
    {
        var pem = _rsaCryptoService.GetPublicKeyPem();
        return Ok(new { publicKeyPem = pem });
    }

    [HttpPost("register")]
    [Consumes("application/json")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] UserRegisterRequest request)
    {
        try
        {
            // Email kontrolü - aynı email ile kayıt olunamaz
            var existingUsers = await _userRepository.GetAllAsync();
            if (existingUsers.Any(u => u.UserEmail == request.UserEmail))
            {
                return BadRequest(new { message = "Bu email adresi zaten kullanılıyor!" });
            }

            // Adres oluştur (opsiyonel)
            Adres? adres = null;
            
            // Frontend'den gelen yeni format veya eski format kullan
            var cityName = request.CityName ?? request.Il;
            var districtName = request.DistrictName ?? request.Ilce;  
            var districtTownshipTownName = request.DistrictTownshipTownName ?? request.SemtBucakBelde;
            var neighbourhoodName = request.NeighbourhoodName ?? request.Mahalle;
            var addressDetails = request.AddressDetails ?? request.AdresDetay;
            
            if (!string.IsNullOrWhiteSpace(cityName) && 
                !string.IsNullOrWhiteSpace(districtName) && 
                !string.IsNullOrWhiteSpace(districtTownshipTownName) && 
                !string.IsNullOrWhiteSpace(neighbourhoodName))
            {
                try
                {
                    adres = await _adresService.CreateAdresAsync(
                        cityName,
                        districtName,
                        districtTownshipTownName,
                        neighbourhoodName,
                        addressDetails
                    );
                }
                catch (Exception ex)
                {
                    // Adres oluşturulamazsa null bırak, kullanıcı kayıt işlemini durdurmayın
                    adres = null;
                }
            }

            var entity = _mapper.Map<User>(request);
            
            // Adres ID'sini set et (eğer adres oluşturulduysa)
            entity.AddressId = adres?.Id;
            
            // UserAddress alanını boş bırak çünkü AddressId kullanıyoruz
            entity.UserAddress = string.Empty;
            
            // Şifre hashleme işlemi - frontend'den hash'lenmiş geliyorsa direkt kullan
            if (request.UserPassword?.Length == 44) // Base64 SHA-256 hash
            {
                entity.UserPassword = request.UserPassword;
            }
            else
            {
                entity.UserPassword = _passwordService.HashPassword(request.UserPassword);
            }
            
            var created = await _userRepository.CreateAsync(entity);
            
            // Yeni kullanıcıya otomatik olarak User rolü ekle
            await _userService.AddRoleToUserAsync(created.Id, "user");
            
            // Kullanıcıyı rollerle birlikte yeniden yükle
            var userWithRoles = await _userService.GetUserWithRolesAsync(created.Id);
            if (userWithRoles == null)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı oluşturuldu ancak roller yüklenemedi!"
                });
            }
            
            var response = _mapper.Map<UserResponse>(userWithRoles);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(userWithRoles);
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Kullanıcı başarıyla kaydedildi!",
                User = response,
                Token = token
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Kayıt işlemi başarısız!",
                Error = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromForm] string userEmail, [FromForm] string userPassword)
    {
        try
        {
            Console.WriteLine($"=== LOGIN ATTEMPT ===");
            Console.WriteLine($"Email: {userEmail}");
            Console.WriteLine($"Password length: {userPassword?.Length ?? 0}");
            
            string providedPassword = userPassword;
            Console.WriteLine($"Final password length: {providedPassword?.Length ?? 0}");
            
            // Email ile kullanıcıyı rollerle birlikte bul
            var user = await _userService.GetUserWithRolesByEmailAsync(userEmail);
            Console.WriteLine($"User found: {user != null}");
            if (user != null)
            {
                Console.WriteLine($"User email: {user.UserEmail}");
                Console.WriteLine($"User active: {user.IsActive}");
                Console.WriteLine($"Stored password hash length: {user.UserPassword?.Length ?? 0}");
            }
            
            // Kullanıcı bulunamadıysa veya şifre yanlışsa
            if (user == null)
            {
                Console.WriteLine($"LOGIN FAILED: User not found");
                
                // Başarısız login logu
                await _activityLogService.LogActivityAsync(
                    userId: 0, // Kullanıcı bulunamadığı için 0
                    activityType: "login_failed",
                    description: $"Başarısız giriş denemesi: {userEmail}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    isSuccessful: false,
                    errorMessage: "Kullanıcı bulunamadı"
                );
                
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Email veya şifre hatalı!"
                });
            }
            
            var passwordVerified = _passwordService.VerifyPassword(providedPassword, user.UserPassword);
            Console.WriteLine($"Password verification result: {passwordVerified}");
            
            if (!passwordVerified)
            {
                Console.WriteLine($"LOGIN FAILED: Password verification failed");
                
                // Başarısız login logu
                await _activityLogService.LogActivityAsync(
                    userId: user.Id,
                    activityType: "login_failed",
                    description: $"Başarısız giriş denemesi: {userEmail}",
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers["User-Agent"].ToString(),
                    isSuccessful: false,
                    errorMessage: "Şifre yanlış"
                );
                
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Email veya şifre hatalı!"
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Hesabınız aktif değil!"
                });
            }

            var response = _mapper.Map<UserResponse>(user);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(user);
            
            // Başarılı login logu
            await _activityLogService.LogActivityAsync(
                userId: user.Id,
                activityType: "login_success",
                description: $"Başarılı giriş: {userEmail}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString(),
                isSuccessful: true
            );
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Giriş başarılı!",
                User = response,
                Token = token
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Giriş işlemi başarısız!",
                Error = ex.Message
            });
        }
    }

    [HttpPost("google-login")]
    public async Task<ActionResult<AuthResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            // Google ile kullanıcıyı doğrula
            var user = await _googleAuthService.AuthenticateGoogleUserAsync(request.AccessToken);
            
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Google ile giriş başarısız!"
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Hesabınız aktif değil!"
                });
            }

            var response = _mapper.Map<UserResponse>(user);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(user);
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Google ile giriş başarılı!",
                User = response,
                Token = token
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Google ile giriş işlemi başarısız!",
                Error = ex.Message
            });
        }
    }

    [HttpPost("logout")]
    public ActionResult<AuthResponse> Logout()
    {
        // JWT token blacklist'e eklenebilir
        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Çıkış başarılı!"
        });
    }

    [HttpGet("trello-login")]
    public ActionResult<string> TrelloLogin()
    {
        try
        {
            var authUrl = _trelloAuthService.GetAuthorizationUrl();
            return Ok(new { authUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Trello giriş URL'i oluşturulamadı!", error = ex.Message });
        }
    }

    [HttpPost("trello-test")]
    public async Task<ActionResult<AuthResponse>> TrelloTest([FromBody] TrelloLoginRequest request)
    {
        try
        {
            Console.WriteLine($"Trello test endpoint called with token: {request.Token?.Substring(0, Math.Min(10, request.Token?.Length ?? 0))}...");
            Console.WriteLine($"Request body: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            if (string.IsNullOrEmpty(request.Token))
            {
                Console.WriteLine("Token is null or empty!");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Trello token bulunamadı!"
                });
            }
            
            // Trello ile kullanıcıyı doğrula
            var user = await _trelloAuthService.AuthenticateTrelloUserAsync(request.Token);
            
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Trello ile giriş başarısız!"
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Hesabınız aktif değil!"
                });
            }

            var response = _mapper.Map<UserResponse>(user);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(user);
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Trello ile giriş başarılı!",
                User = response,
                Token = token
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Trello test error: {ex.Message}");
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Trello ile giriş işlemi başarısız!",
                Error = ex.Message
            });
        }
    }

    [HttpPost("trello-callback")]
    public async Task<ActionResult<AuthResponse>> TrelloCallback([FromBody] TrelloLoginRequest request)
    {
        try
        {
            Console.WriteLine($"Trello callback received - Token: {request.Token?.Substring(0, Math.Min(10, request.Token?.Length ?? 0))}...");
            Console.WriteLine($"Request body: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            if (string.IsNullOrEmpty(request.Token))
            {
                Console.WriteLine("Token is null or empty!");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Trello token bulunamadı!"
                });
            }
            
            // Trello ile kullanıcıyı doğrula
            var user = await _trelloAuthService.AuthenticateTrelloUserAsync(request.Token);
            
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Trello ile giriş başarısız!"
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Hesabınız aktif değil!"
                });
            }

            var response = _mapper.Map<UserResponse>(user);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(user);
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Trello ile giriş başarılı!",
                User = response,
                Token = token
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Trello ile giriş işlemi başarısız!",
                Error = ex.Message
            });
        }
    }

    [HttpGet("jira-login")]
    public ActionResult<string> JiraLogin()
    {
        try
        {
            var authUrl = _jiraAuthService.GetAuthorizationUrl();
            return Ok(new { authUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Jira giriş URL'i oluşturulamadı!", error = ex.Message });
        }
    }

    [HttpPost("jira-test")]
    public async Task<ActionResult<AuthResponse>> JiraTest([FromBody] JiraLoginRequest request)
    {
        try
        {
            Console.WriteLine($"Jira test endpoint called with code: {request.Code?.Substring(0, Math.Min(10, request.Code?.Length ?? 0))}...");
            Console.WriteLine($"Request body: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            if (string.IsNullOrEmpty(request.Code))
            {
                Console.WriteLine("Jira code is null or empty!");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Jira authorization code bulunamadı!"
                });
            }
            
            // Jira ile kullanıcıyı doğrula
            var user = await _jiraAuthService.AuthenticateJiraUserAsync(request.Code);
            
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Jira ile giriş başarısız!"
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Hesabınız aktif değil!"
                });
            }

            var response = _mapper.Map<UserResponse>(user);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(user);
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Jira ile giriş başarılı!",
                User = response,
                Token = token
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Jira test error: {ex.Message}");
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Jira ile giriş işlemi başarısız!",
                Error = ex.Message
            });
        }
    }

    [HttpPost("jira-callback")]
    public async Task<ActionResult<AuthResponse>> JiraCallback([FromBody] JiraLoginRequest request)
    {
        try
        {
            Console.WriteLine($"=== JIRA CALLBACK START ===");
            Console.WriteLine($"Jira callback received - Code: {request.Code?.Substring(0, Math.Min(10, request.Code?.Length ?? 0))}...");
            Console.WriteLine($"Code length: {request.Code?.Length ?? 0}");
            Console.WriteLine($"Request body: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            if (string.IsNullOrEmpty(request.Code))
            {
                Console.WriteLine("Jira code is null or empty!");
                Console.WriteLine($"=== JIRA CALLBACK FAILED - EMPTY CODE ===");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Jira authorization code bulunamadı!"
                });
            }
            
            Console.WriteLine("Calling JiraAuthService.AuthenticateJiraUserAsync...");
            // Jira ile kullanıcıyı doğrula
            var user = await _jiraAuthService.AuthenticateJiraUserAsync(request.Code);
            
            if (user == null)
            {
                Console.WriteLine("Jira authentication failed - user is null");
                Console.WriteLine($"=== JIRA CALLBACK FAILED - AUTHENTICATION FAILED ===");
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Jira ile giriş başarısız!"
                });
            }

            if (!user.IsActive)
            {
                Console.WriteLine("User is not active");
                Console.WriteLine($"=== JIRA CALLBACK FAILED - USER NOT ACTIVE ===");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Hesabınız aktif değil!"
                });
            }

            Console.WriteLine($"Jira authentication successful - User ID: {user.Id}, Email: {user.UserEmail}");
            var response = _mapper.Map<UserResponse>(user);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(user);
            
            Console.WriteLine($"JWT token generated successfully");
            Console.WriteLine($"=== JIRA CALLBACK SUCCESS ===");
            
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Jira ile giriş başarılı!",
                User = response,
                Token = token
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Jira callback error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine($"=== JIRA CALLBACK ERROR ===");
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Jira ile giriş işlemi başarısız!",
                Error = ex.Message
            });
        }
    }
}
