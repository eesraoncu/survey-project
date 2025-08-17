using AutoMapper;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IMapper _mapper;

    public UserSettingsService(IUserSettingsRepository userSettingsRepository, IMapper mapper)
    {
        _userSettingsRepository = userSettingsRepository;
        _mapper = mapper;
    }

    public async Task<UserSettingsResponse?> GetUserSettingsAsync(int userId)
    {
        var userSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
        return _mapper.Map<UserSettingsResponse>(userSettings);
    }

    public async Task<UserSettingsResponse> CreateUserSettingsAsync(UserSettingsRequest userSettingsRequest, int userId)
    {
        var userSettings = _mapper.Map<UserSettings>(userSettingsRequest);
        userSettings.UserId = userId;
        userSettings.CreatedAt = DateTime.UtcNow;
        userSettings.UpdatedAt = DateTime.UtcNow;

        var createdSettings = await _userSettingsRepository.CreateAsync(userSettings);
        return _mapper.Map<UserSettingsResponse>(createdSettings);
    }

    public async Task<UserSettingsResponse> UpdateUserSettingsAsync(UserSettingsUpdateRequest userSettingsUpdateRequest, int userId)
    {
        var existingSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
        if (existingSettings == null)
            throw new InvalidOperationException("User settings not found");

        if (existingSettings.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own settings");

        var userSettings = _mapper.Map<UserSettings>(userSettingsUpdateRequest);
        userSettings.UserId = userId;
        userSettings.CreatedAt = existingSettings.CreatedAt;
        userSettings.UpdatedAt = DateTime.UtcNow;

        var updatedSettings = await _userSettingsRepository.UpdateAsync(userSettings);
        return _mapper.Map<UserSettingsResponse>(updatedSettings);
    }

    public async Task<bool> DeleteUserSettingsAsync(int id, int userId)
    {
        var existingSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
        if (existingSettings == null)
            return false;

        if (existingSettings.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own settings");

        return await _userSettingsRepository.DeleteAsync(id);
    }

    public async Task<bool> UserSettingsExistsAsync(int id)
    {
        return await _userSettingsRepository.ExistsAsync(id);
    }
}
