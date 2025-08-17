using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SurveyApp.Services;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using System.Security.Claims;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserSettingsController : ControllerBase
{
    private readonly IUserSettingsService _userSettingsService;

    public UserSettingsController(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }

    [HttpGet]
    public async Task<ActionResult<UserSettingsResponse>> GetUserSettings()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userSettings = await _userSettingsService.GetUserSettingsAsync(currentUserId);
            
            if (userSettings == null)
                return NotFound(new { message = "User settings not found" });

            return Ok(userSettings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserSettingsResponse>> CreateUserSettings(UserSettingsRequest userSettingsRequest)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var createdSettings = await _userSettingsService.CreateUserSettingsAsync(userSettingsRequest, currentUserId);
            return CreatedAtAction(nameof(GetUserSettings), createdSettings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult<UserSettingsResponse>> UpdateUserSettings(UserSettingsUpdateRequest userSettingsUpdateRequest)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var updatedSettings = await _userSettingsService.UpdateUserSettingsAsync(userSettingsUpdateRequest, currentUserId);
            return Ok(updatedSettings);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUserSettings(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var deleted = await _userSettingsService.DeleteUserSettingsAsync(id, currentUserId);
            
            if (!deleted)
                return NotFound(new { message = "User settings not found" });

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
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
