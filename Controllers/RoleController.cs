using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RoleController : ControllerBase
{
    private readonly IUserService _userService;

    public RoleController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("add-owner-role")]
    public async Task<ActionResult> AddOwnerRole()
    {
        try
        {
            // JWT token'dan kullanıcı ID'sini al
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı!" });
            }

            // Kullanıcıya Owner rolü ekle
            var success = await _userService.AddRoleToUserAsync(userId, "owner");
            
            if (success)
            {
                return Ok(new { 
                    success = true, 
                    message = "Owner rolü başarıyla eklendi!",
                    roles = new[] { "User", "Owner" }
                });
            }
            else
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Owner rolü eklenemedi!" 
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = "Rol ekleme işlemi başarısız!", 
                error = ex.Message 
            });
        }
    }

    [HttpPost("check-owner-role")]
    public async Task<ActionResult> CheckOwnerRole()
    {
        try
        {
            // JWT token'dan kullanıcı ID'sini al
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı!" });
            }

            // Kullanıcının Owner rolü var mı kontrol et
            var hasOwnerRole = await _userService.UserHasRoleAsync(userId, "owner");
            
            return Ok(new { 
                success = true, 
                hasOwnerRole = hasOwnerRole,
                message = hasOwnerRole ? "Kullanıcının Owner rolü var!" : "Kullanıcının Owner rolü yok!"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = "Rol kontrolü başarısız!", 
                error = ex.Message 
            });
        }
    }
}
