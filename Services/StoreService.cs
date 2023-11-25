using MongoDB.Driver;
using RetroCarsWebApp.Data;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class StoreService
{
    private readonly IMongoCollection<Car> _storeCollection;
    
    public StoreService(string connectionString, string dbName)
    {
        var client = new MongoDbContext(connectionString, dbName);
        _storeCollection = client.GetCollection<Car>("StoreCars");
    }
    
    public Car? GetCar(string id) =>
        _storeCollection.FindAsync(car => car.Id == id).Result.FirstOrDefault();
    
    public IEnumerable<Car> GetCars() =>
        new List<Car>(_storeCollection.FindAsync(car => true).Result.ToList());
    
    public IEnumerable<Car> GetCars(int page, int pageSize) =>
        new List<Car>(_storeCollection.Find(car => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList()
        );

}