using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Role
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("role_name")]
    public string RoleName { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
} 