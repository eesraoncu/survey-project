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
    private readonly IAddressService _addressService;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthController(
        IUserRepository userRepository, 
        IAddressService addressService, 
        IPasswordService passwordService,
        IJwtService jwtService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _addressService = addressService;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    [HttpPost("register")]
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
            Address? address = null;
            if (!string.IsNullOrWhiteSpace(request.CityName) && 
                !string.IsNullOrWhiteSpace(request.DistrictName) && 
                !string.IsNullOrWhiteSpace(request.DistrictTownshipTownName) && 
                !string.IsNullOrWhiteSpace(request.NeighbourhoodName))
            {
                address = await _addressService.CreateAddressAsync(
                    request.CityName,
                    request.DistrictName,
                    request.DistrictTownshipTownName,
                    request.NeighbourhoodName,
                    request.AddressDetails
                );
            }

            var entity = _mapper.Map<User>(request);
            
            // Adres ID'sini set et (eğer adres oluşturulduysa)
            entity.AddressId = address?.Id;
            
            // Yeni kullanıcılar otomatik olarak user rolü (3) alır
            entity.RoleId = 3; // User role
            
            // Şifre hashleme işlemi
            entity.UserPassword = _passwordService.HashPassword(request.UserPassword);
            
            var created = await _userRepository.CreateAsync(entity);
            var response = _mapper.Map<UserResponse>(created);
            
            // JWT token oluştur
            var token = _jwtService.GenerateToken(created);
            
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
    public async Task<ActionResult<AuthResponse>> Login([FromBody] UserLoginRequest request)
    {
        try
        {
            // Email ile kullanıcı bul
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.UserEmail == request.UserEmail);
            
            // Kullanıcı bulunamadıysa veya şifre yanlışsa
            if (user == null || !_passwordService.VerifyPassword(request.UserPassword, user.UserPassword))
            {
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
}
