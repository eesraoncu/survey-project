using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("event_date")]
    public DateTime EventDate { get; set; }

    [BsonElement("start_time")]
    public TimeSpan StartTime { get; set; }

    [BsonElement("end_time")]
    public TimeSpan EndTime { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UserId { get; set; }

    [BsonElement("is_all_day")]
    public bool IsAllDay { get; set; } = false;

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("color")]
    public string Color { get; set; } = "#3788d8";

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}
