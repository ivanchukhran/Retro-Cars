namespace RetroCarsWebApp.Models;

public class Car
{
    public string Id { get; set; }
    public string OwnerId { get; set; }
    public string Model { get; set; }
    public string Mark { get; set; }
    public string? BuyingPrice { get; set; }
    public string? MaintenanceCost { get; set; }
    public string? LuggageCapacity { get; set; }
    public string? SafetyRating { get; set; }
    public string? CarClass { get; set; }
    public string? NumberOfDoors { get; set; }
    public string? NumberOfSeats { get; set; }
    public int Year { get; set; }
    public bool IsAvailableOnStore { get; set; }
    
    public override string ToString()
    {
        return $"Id: {Id}, OwnerId: {OwnerId}, Model: {Model}, Mark: {Mark}, BuyingPrice: {BuyingPrice}, MaintenanceCost: {MaintenanceCost}, LuggageCapacity: {LuggageCapacity}, SafetyRating: {SafetyRating}, CarClass: {CarClass}, NumberOfDoors: {NumberOfDoors}, NumberOfSeats: {NumberOfSeats}, Year: {Year}, IsAvailableOnStore: {IsAvailableOnStore}";
    }
}