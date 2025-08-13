using Microsoft.AspNetCore.Mvc;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RolesController(IRoleRepository roleRepository)
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

    [HttpPost]
    public async Task<ActionResult<Role>> Create([FromBody] Role role)
    {
        var created = await _roleRepository.CreateAsync(role);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role role)
    {
        var existing = await _roleRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
        
        role.Id = id; // ID'yi koru
        var ok = await _roleRepository.UpdateAsync(id, role);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _roleRepository.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
