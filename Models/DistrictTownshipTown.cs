using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class SemtBucakBelde
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("id")]
    public int SemtId { get; set; }

    [BsonElement("semt_bucak_belde")]
    public string SemtBucakBeldeAdi { get; set; } = string.Empty;

    [BsonElement("ilce_id")]
    public int IlceId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 