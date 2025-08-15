using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class UserRole
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UserId { get; set; }

    [BsonElement("role_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int RoleId { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}
