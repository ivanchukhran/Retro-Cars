using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RetroCarsWebApp.Models;

public class Car
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? OwnerId { get; set; }
    public string Model { get; set; }
    public string Mark { get; set; }
    public string BuyingPrice { get; set; }
    public string MaintenanceCost { get; set; }
    public string LuggageCapacity { get; set; }
    public string SafetyRating { get; set; }
    public string CarClass { get; set; }
    public int NumberOfDoors { get; set; }
    public int NumberOfSeats { get; set; }
    public int Year { get; set; }
    [BsonRepresentation(BsonType.Boolean)]
    public bool IsAvailableOnStore { get; set; }
}