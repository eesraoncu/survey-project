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
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileResponse>> GetUserProfile()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userProfile = await _userProfileService.GetUserProfileAsync(currentUserId);
            
            if (userProfile == null)
                return NotFound(new { message = "User profile not found" });

            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserProfileResponse>> CreateUserProfile(UserProfileRequest userProfileRequest)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var createdProfile = await _userProfileService.CreateUserProfileAsync(userProfileRequest, currentUserId);
            return CreatedAtAction(nameof(GetUserProfile), createdProfile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult<UserProfileResponse>> UpdateUserProfile(UserProfileUpdateRequest userProfileUpdateRequest)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var updatedProfile = await _userProfileService.UpdateUserProfileAsync(userProfileUpdateRequest, currentUserId);
            return Ok(updatedProfile);
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
    public async Task<ActionResult> DeleteUserProfile(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var deleted = await _userProfileService.DeleteUserProfileAsync(id, currentUserId);
            
            if (!deleted)
                return NotFound(new { message = "User profile not found" });

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
