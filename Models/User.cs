using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("user_name")]
    public string UserName { get; set; } = string.Empty;

    [BsonElement("user_surname")]
    public string UserSurname { get; set; } = string.Empty;

    [BsonElement("user_email")]
    public string UserEmail { get; set; } = string.Empty;

    [BsonElement("user_password")]
    public string UserPassword { get; set; } = string.Empty;

    [BsonElement("user_address")]
    public string UserAddress { get; set; } = string.Empty;

    [BsonElement("user_age")]
    public int UserAge { get; set; }

    [BsonElement("address_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int AddressId { get; set; } // Foreign Key

    [BsonElement("role_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int RoleId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
} 