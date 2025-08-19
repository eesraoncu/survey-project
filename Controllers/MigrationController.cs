using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;
using SurveyApp.Infrastructure.Repositories;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MigrationController : ControllerBase
{
    private readonly MigrationService _migrationService;
    private readonly IRoleRepository _roleRepository;
    private readonly IQuestionTypeRepository _questionTypeRepository;

    public MigrationController(MigrationService migrationService, IRoleRepository roleRepository, IQuestionTypeRepository questionTypeRepository)
    {
        _migrationService = migrationService;
        _roleRepository = roleRepository;
        _questionTypeRepository = questionTypeRepository;
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

    [HttpPost("create-default-roles")]
    public async Task<ActionResult> CreateDefaultRoles()
    {
        try
        {
            // Mevcut rolleri kontrol et
            var existingRoles = await _roleRepository.GetAllAsync();
            var results = new List<object>();

            // Admin rolü (ID: 1)
            var adminExists = existingRoles.Any(r => r.Id == 1 && r.RoleName == "admin");
            if (!adminExists)
            {
                var adminRole = new Models.Role { Id = 1, RoleName = "admin" };
                await _roleRepository.CreateAsync(adminRole);
                results.Add(new { name = "admin", id = 1, status = "created" });
            }
            else
            {
                results.Add(new { name = "admin", id = 1, status = "already exists" });
            }

            // Owner rolü (ID: 2) 
            var ownerExists = existingRoles.Any(r => r.Id == 2 && r.RoleName == "owner");
            if (!ownerExists)
            {
                var ownerRole = new Models.Role { Id = 2, RoleName = "owner" };
                await _roleRepository.CreateAsync(ownerRole);
                results.Add(new { name = "owner", id = 2, status = "created" });
            }
            else
            {
                results.Add(new { name = "owner", id = 2, status = "already exists" });
            }

            // User rolü (ID: 3)
            var userExists = existingRoles.Any(r => r.Id == 3 && r.RoleName == "user");
            if (!userExists)
            {
                var userRole = new Models.Role { Id = 3, RoleName = "user" };
                await _roleRepository.CreateAsync(userRole);
                results.Add(new { name = "user", id = 3, status = "created" });
            }
            else
            {
                results.Add(new { name = "user", id = 3, status = "already exists" });
            }

            return Ok(new { 
                success = true, 
                message = "Roller kontrol edildi ve gerekiyorsa oluşturuldu!",
                roles = results
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = "Roller oluşturulamadı!", 
                error = ex.Message 
            });
        }
    }



    [HttpPost("create-default-question-types")]
    public async Task<ActionResult> CreateDefaultQuestionTypes()
    {
        try
        {
            var defaultTypes = new[]
            {
                new { Name = "Kısa Yanıt", Code = "short_text", RequiresChoices = false, AllowsMultiple = false },
                new { Name = "Paragraf", Code = "paragraph", RequiresChoices = false, AllowsMultiple = false },
                new { Name = "Çoktan Seçmeli", Code = "multiple_choice", RequiresChoices = true, AllowsMultiple = false },
                new { Name = "Çoklu Seçim", Code = "multi_select", RequiresChoices = true, AllowsMultiple = true },
                new { Name = "Açılır Liste", Code = "dropdown", RequiresChoices = true, AllowsMultiple = false }
            };

            var results = new List<object>();
            foreach (var type in defaultTypes)
            {
                var existing = await _questionTypeRepository.GetByCodeAsync(type.Code);
                if (existing == null)
                {
                    var questionType = new Models.QuestionType 
                    { 
                        QuestionTypeName = type.Name,
                        QuestionTypeCode = type.Code,
                        RequiresChoices = type.RequiresChoices,
                        AllowsMultipleSelection = type.AllowsMultiple,
                        IsActive = true
                    };
                    var created = await _questionTypeRepository.CreateAsync(questionType);
                    results.Add(new { name = type.Name, code = type.Code, id = created.Id, status = "created" });
                }
                else
                {
                    results.Add(new { name = type.Name, code = type.Code, id = existing.Id, status = "already exists" });
                }
            }

            return Ok(new { 
                success = true, 
                message = "Soru türleri kontrol edildi ve gerekiyorsa oluşturuldu!",
                questionTypes = results
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = "Soru türleri oluşturulamadı!", 
                error = ex.Message 
            });
        }
    }
}
