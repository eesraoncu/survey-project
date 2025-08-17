using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class UserProfile
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UserId { get; set; }

    [BsonElement("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("last_name")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone_number")]
    public string? PhoneNumber { get; set; }

    [BsonElement("profile_picture")]
    public string? ProfilePicture { get; set; }

    [BsonElement("bio")]
    public string? Bio { get; set; }

    [BsonElement("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [BsonElement("gender")]
    public string? Gender { get; set; } // male, female, other, prefer_not_to_say

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("website")]
    public string? Website { get; set; }

    [BsonElement("social_media")]
    public SocialMediaLinks? SocialMedia { get; set; }

    [BsonElement("interests")]
    public List<string> Interests { get; set; } = new List<string>();

    [BsonElement("skills")]
    public List<string> Skills { get; set; } = new List<string>();

    [BsonElement("education")]
    public List<Education> Education { get; set; } = new List<Education>();

    [BsonElement("experience")]
    public List<Experience> Experience { get; set; } = new List<Experience>();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class SocialMediaLinks
{
    [BsonElement("linkedin")]
    public string? LinkedIn { get; set; }

    [BsonElement("twitter")]
    public string? Twitter { get; set; }

    [BsonElement("facebook")]
    public string? Facebook { get; set; }

    [BsonElement("instagram")]
    public string? Instagram { get; set; }

    [BsonElement("github")]
    public string? GitHub { get; set; }
}

public class Education
{
    [BsonElement("institution")]
    public string Institution { get; set; } = string.Empty;

    [BsonElement("degree")]
    public string Degree { get; set; } = string.Empty;

    [BsonElement("field_of_study")]
    public string FieldOfStudy { get; set; } = string.Empty;

    [BsonElement("start_date")]
    public DateTime StartDate { get; set; }

    [BsonElement("end_date")]
    public DateTime? EndDate { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }
}

public class Experience
{
    [BsonElement("company")]
    public string Company { get; set; } = string.Empty;

    [BsonElement("position")]
    public string Position { get; set; } = string.Empty;

    [BsonElement("start_date")]
    public DateTime StartDate { get; set; }

    [BsonElement("end_date")]
    public DateTime? EndDate { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("is_current")]
    public bool IsCurrent { get; set; } = false;
}
