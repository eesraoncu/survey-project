using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Services;
using SurveyApp.Models;
using AutoMapper;
using System.Security.Claims;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IPasswordService passwordService, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var list = await _userRepository.GetAllAsync();
        return Ok(_mapper.Map<List<UserResponse>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var entity = await _userRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<UserResponse>(entity));
    }

    [HttpGet("by-role/{roleId}")]
    public async Task<ActionResult<List<UserResponse>>> GetByRoleId(int roleId)
    {
        // Bu metod artık UserService üzerinden çalışacak
        // Şimdilik boş liste döndür
        return Ok(new List<UserResponse>());
    }

    [HttpGet("by-address/{addressId}")]
    public async Task<ActionResult<List<UserResponse>>> GetByAddressId(int addressId)
    {
        var list = await _userRepository.GetByAddressIdAsync(addressId);
        return Ok(_mapper.Map<List<UserResponse>>(list));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateRequest request)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        
        // RoleId güncellenemez - sadece sistem tarafından değiştirilebilir
        _mapper.Map(request, existing);
        var ok = await _userRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _userRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    // Admin rolü güncelleme - sadece sistem tarafından kullanılır
    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateRequest request)
    {
        // Bu metod artık UserService üzerinden çalışacak
        // Şimdilik NotImplemented döndür
        return StatusCode(501, new { message = "Bu özellik henüz implement edilmedi!" });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Şifre eşleşme kontrolü
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Yeni şifre ve şifre onayı eşleşmiyor" });
            }

            // Mevcut kullanıcıyı al
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı" });
            }

            // Mevcut şifreyi doğrula
            if (!_passwordService.VerifyPassword(request.CurrentPassword, user.UserPassword))
            {
                return BadRequest(new { message = "Mevcut şifre yanlış" });
            }

            // Yeni şifreyi hash'le
            user.UserPassword = _passwordService.HashPassword(request.NewPassword);
            
            // Kullanıcıyı güncelle
            var updated = await _userRepository.UpdateAsync(currentUserId, user);
            if (!updated)
            {
                return StatusCode(500, new { message = "Şifre güncellenirken hata oluştu" });
            }

            return Ok(new { message = "Şifre başarıyla güncellendi" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return userId;
        
        throw new InvalidOperationException("User ID not found in claims");
    }
}


