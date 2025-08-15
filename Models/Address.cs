using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Adres
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("mahalle_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int MahalleId { get; set; } // Foreign Key

    [BsonElement("adres_detay")]
    public string AdresDetay { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 