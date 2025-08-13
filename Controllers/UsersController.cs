using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using AutoMapper;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
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
        var list = await _userRepository.GetByRoleIdAsync(roleId);
        return Ok(_mapper.Map<List<UserResponse>>(list));
    }

    [HttpGet("by-address/{addressId}")]
    public async Task<ActionResult<List<UserResponse>>> GetByAddressId(int addressId)
    {
        var list = await _userRepository.GetByAddressIdAsync(addressId);
        return Ok(_mapper.Map<List<UserResponse>>(list));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserRegisterRequest request)
    {
        var entity = _mapper.Map<User>(request);
        // Not: Parola hashleme işlemini normalde burada yapmalısınız.
        var created = await _userRepository.CreateAsync(entity);
        var response = _mapper.Map<UserResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateRequest request)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();
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
}


