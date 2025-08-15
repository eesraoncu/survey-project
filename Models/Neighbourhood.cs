using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Mahalle
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("mahalle")]
    public string MahalleAdi { get; set; } = string.Empty;

    [BsonElement("mahalle_aciklama")]
    public string MahalleAciklama { get; set; } = string.Empty;

    [BsonElement("semt_bucak_belde_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SemtBucakBeldeId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 