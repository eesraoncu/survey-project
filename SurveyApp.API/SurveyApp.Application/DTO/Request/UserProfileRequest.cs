namespace SurveyApp.Application.DTO.Request;

public class UserProfileRequest
{
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
    public SocialMediaLinksRequest? SocialMedia { get; set; }
    public List<string> Interests { get; set; } = new List<string>();
    public List<string> Skills { get; set; } = new List<string>();
    public List<EducationRequest> Education { get; set; } = new List<EducationRequest>();
    public List<ExperienceRequest> Experience { get; set; } = new List<ExperienceRequest>();
}

public class SocialMediaLinksRequest
{
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? GitHub { get; set; }
}

public class EducationRequest
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
}

public class ExperienceRequest
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsCurrent { get; set; } = false;
}

public class UserProfileUpdateRequest : UserProfileRequest
{
    public int Id { get; set; }
}
