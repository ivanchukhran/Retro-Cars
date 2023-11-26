using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RetroCarsWebApp.Models;

public class Session
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}