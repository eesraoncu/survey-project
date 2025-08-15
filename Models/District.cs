using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Ilce
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("id")]
    public int IlceId { get; set; }

    [BsonElement("ilce")]
    public string IlceAdi { get; set; } = string.Empty;

    [BsonElement("il_id")]
    public int IlId { get; set; } // Foreign Key
} 