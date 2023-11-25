using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace RetroCarsWebApp.Data;

public class MongoDbContext : DbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(string connectionString, string dbName)
    {
        var client = new MongoClient($"{connectionString}");
        _database = client.GetDatabase(dbName);
        Console.WriteLine($"{dbName} database connected at {connectionString}.");
    }
    
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}