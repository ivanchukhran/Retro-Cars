using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using MongoDB.Driver;
using RetroCarsWebApp.Data;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class CarService
{
    private readonly IMongoCollection<Car> _cars;

    public CarService(string connectionString, string dbName)
    {
        var client = new MongoDbContext(connectionString, dbName);
        _cars = client.GetCollection<Car>("Cars");
    }

    public Car CreateCar(Car car)
    {
        _cars.InsertOne(car);
        return car;
    }
    
    public IEnumerable<Car> GetUserCars(string userId) =>
        new List<Car>(_cars.FindAsync(car => car.OwnerId == userId).Result.ToList());

    public IEnumerable<Car> GetUserCarsPaginated(string userId, int page, int pageSize = 10)
    {
        if (page < 1) page = 1;
        return new List<Car>(
            _cars.Find(car => car.OwnerId == userId)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList());
    }
    
    public IEnumerable<Car> GetStoreCars(string userId) => 
        new List<Car>(_cars.FindAsync(car => car.OwnerId != userId).Result.ToList());

    public IEnumerable<Car> GetStoreCarsPaginated(int page = 1, int pageSize = 10) =>
        new List<Car>(_cars.Find(car => car.IsAvailableOnStore == true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList()
        );

    public Car? GetCar(string id) =>
        _cars.FindAsync(car => car.Id == id).Result.FirstOrDefault();

    public void UpdateCar(Car car) =>
        _cars.ReplaceOneAsync(c => c.Id == car.Id, car).Wait();

    public void DeleteCar(string id) =>
        _cars.DeleteOneAsync(car => car.Id == id).Wait();

    public IEnumerable<Car> GetCars() =>
        new List<Car>(_cars.FindAsync(car => true).Result.ToList());

    public IEnumerable<Car> GetCars(int page, int pageSize)
    {
        if (page < 1)
        {
            Console.WriteLine("Page number cannot be less than 1: {0}", page);
            page = 1;
        }
        return new List<Car>(_cars.Find(car => true).Skip((page - 1) * pageSize).Limit(pageSize).ToList());
    }
}