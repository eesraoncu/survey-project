using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Admin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("admin_name")]
    public string AdminName { get; set; } = string.Empty;

    [BsonElement("admin_surname")]
    public string AdminSurname { get; set; } = string.Empty;

    [BsonElement("admin_email")]
    public string AdminEmail { get; set; } = string.Empty;

    [BsonElement("admin_password")]
    public string AdminPassword { get; set; } = string.Empty;

    [BsonElement("role_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RoleId { get; set; } = string.Empty; // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
} 