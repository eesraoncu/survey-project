using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Il
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("id")]
    public int IlId { get; set; }

    [BsonElement("il")]
    public string IlAdi { get; set; } = string.Empty;
} 