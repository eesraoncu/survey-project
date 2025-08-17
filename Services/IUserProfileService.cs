using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Services;

public interface IUserProfileService
{
    Task<UserProfileResponse?> GetUserProfileAsync(int userId);
    Task<UserProfileResponse> CreateUserProfileAsync(UserProfileRequest userProfileRequest, int userId);
    Task<UserProfileResponse> UpdateUserProfileAsync(UserProfileUpdateRequest userProfileUpdateRequest, int userId);
    Task<bool> DeleteUserProfileAsync(int id, int userId);
    Task<bool> UserProfileExistsAsync(int id);
}
