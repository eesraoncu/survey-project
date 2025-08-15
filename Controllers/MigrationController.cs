using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MigrationController : ControllerBase
{
    private readonly MigrationService _migrationService;

    public MigrationController(MigrationService migrationService)
    {
        _migrationService = migrationService;
    }

    [HttpPost("migrate-users")]
    public async Task<ActionResult> MigrateUsers()
    {
        try
        {
            await _migrationService.MigrateUsersToNewRoleSystem();
            
            return Ok(new { 
                success = true, 
                message = "Kullanıcılar başarıyla yeni rol sistemine geçirildi!" 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = "Migration başarısız!", 
                error = ex.Message 
            });
        }
    }
}
