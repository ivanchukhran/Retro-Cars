using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RetroCarsWebApp.Models;

public class Session
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; }
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime ExpiresAt { get; set; }
}