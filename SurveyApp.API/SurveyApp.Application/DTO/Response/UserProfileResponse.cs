namespace SurveyApp.Application.DTO.Response;

public class UserProfileResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public SocialMediaLinksResponse? SocialMedia { get; set; }
    public List<string> Interests { get; set; } = new List<string>();
    public List<string> Skills { get; set; } = new List<string>();
    public List<EducationResponse> Education { get; set; } = new List<EducationResponse>();
    public List<ExperienceResponse> Experience { get; set; } = new List<ExperienceResponse>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SocialMediaLinksResponse
{
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? GitHub { get; set; }
}

public class EducationResponse
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
}

public class ExperienceResponse
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsCurrent { get; set; }
}
