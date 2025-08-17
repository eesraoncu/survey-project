using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Services;

public interface IUserSettingsService
{
    Task<UserSettingsResponse?> GetUserSettingsAsync(int userId);
    Task<UserSettingsResponse> CreateUserSettingsAsync(UserSettingsRequest userSettingsRequest, int userId);
    Task<UserSettingsResponse> UpdateUserSettingsAsync(UserSettingsUpdateRequest userSettingsUpdateRequest, int userId);
    Task<bool> DeleteUserSettingsAsync(int id, int userId);
    Task<bool> UserSettingsExistsAsync(int id);
}
