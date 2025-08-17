using AutoMapper;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Models;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(int userId)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        return _mapper.Map<UserProfileResponse>(userProfile);
    }

    public async Task<UserProfileResponse> CreateUserProfileAsync(UserProfileRequest userProfileRequest, int userId)
    {
        var userProfile = _mapper.Map<UserProfile>(userProfileRequest);
        userProfile.UserId = userId;
        userProfile.CreatedAt = DateTime.UtcNow;
        userProfile.UpdatedAt = DateTime.UtcNow;

        var createdProfile = await _userProfileRepository.CreateAsync(userProfile);
        return _mapper.Map<UserProfileResponse>(createdProfile);
    }

    public async Task<UserProfileResponse> UpdateUserProfileAsync(UserProfileUpdateRequest userProfileUpdateRequest, int userId)
    {
        var existingProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        if (existingProfile == null)
            throw new InvalidOperationException("User profile not found");

        if (existingProfile.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own profile");

        var userProfile = _mapper.Map<UserProfile>(userProfileUpdateRequest);
        userProfile.UserId = userId;
        userProfile.CreatedAt = existingProfile.CreatedAt;
        userProfile.UpdatedAt = DateTime.UtcNow;

        var updatedProfile = await _userProfileRepository.UpdateAsync(userProfile);
        return _mapper.Map<UserProfileResponse>(updatedProfile);
    }

    public async Task<bool> DeleteUserProfileAsync(int id, int userId)
    {
        var existingProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        if (existingProfile == null)
            return false;

        if (existingProfile.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own profile");

        return await _userProfileRepository.DeleteAsync(id);
    }

    public async Task<bool> UserProfileExistsAsync(int id)
    {
        return await _userProfileRepository.ExistsAsync(id);
    }
}
