using Microsoft.AspNetCore.Mvc;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/role")]
public sealed class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Role>>> GetAll()
    {
        var list = await _roleRepository.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetById(int id)
    {
        var entity = await _roleRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(entity);
    }

    // Role oluşturma kaldırıldı - sadece manuel eklenebilir
    // [HttpPost] - Kaldırıldı

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role role)
    {
        var existing = await _roleRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        
        // Sadece role_name güncellenebilir, ID değiştirilemez
        existing.RoleName = role.RoleName;
        var ok = await _roleRepository.UpdateAsync(id, existing);
        return ok ? NoContent() : NotFound();
    }

    // Role silme kaldırıldı - sistem rolleri silinemez
    // [HttpDelete] - Kaldırıldı
}
