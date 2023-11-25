using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using MongoDB.Driver;
using RetroCarsWebApp.Data;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class UserService 
{
    private readonly IMongoCollection<User> _users;

    public UserService(string connectionString, string dbName)
    {
        var client = new MongoDbContext(connectionString, dbName);
        _users = client.GetCollection<User>("Users");
    }

    public User CreateUser(User user)
    {
        user.Password = HashPassword(user.Password);
        _users.InsertOne(user);
        return user;
    }

    public User? GetUser(string id) => 
        _users.FindAsync(user => user.Id == id).Result.FirstOrDefault();
    
    public User? GetUserByUsernameAndPassword(string username, string password) => 
        _users.FindAsync(user => user.Username == username && user.Password == password).Result.FirstOrDefault();

    public User? GetUserByEmail(string userEmail) => 
        _users.FindAsync(u => u.Email == userEmail).Result.FirstOrDefault();

    public User? GetUserByUsername(string username) => 
        _users.FindAsync(user => user.Username == username).Result.FirstOrDefault();

    public void UpdateUser(User user) => 
        _users.ReplaceOneAsync(u => u.Id == user.Id, user).Wait();

    public void DeleteUser(string id) => 
        _users.DeleteOneAsync(user => user.Id == id).Wait();

    public string HashPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        string hashed = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8)
        );
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];
        string hashed = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8)
        );
        return hash == hashed;
    }
}