using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using MongoDB.Driver;
using NuGet.Protocol;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class UserService
{
    private readonly String _filePath;

    public UserService(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public List<User> GetUsers()
    {
        var jsonString = FileInteractor.ReadAsync(_filePath).Result;
        if (string.IsNullOrEmpty(jsonString))
        {
            FileInteractor.WriteAsync(_filePath, "[]").Wait();
            jsonString = FileInteractor.ReadAsync(_filePath).Result;
        }
        return jsonString.FromJson<List<User>>();
    }

    public User CreateUser(User user)
    {
        user.Id = Guid.NewGuid().ToString();
        user.Password = HashPassword(user.Password);
        FileInteractor.WriteAsync(_filePath, GetUsers().Append(user).ToJson()).Wait();
        return user;
    }

    public User? GetUser(string id) =>
        GetUsers().FirstOrDefault(user => user.Id == id);

    public User? GetUserByUsernameAndPassword(string username, string password) =>
        GetUsers().FirstOrDefault(u => u.Username == username && u.Password == password);

    public User? GetUserByEmail(string userEmail) =>
        GetUsers().FirstOrDefault(u => u.Email == userEmail);

    public User? GetUserByUsername(string username) =>
        GetUsers().FirstOrDefault(u => u.Username == username);

    public void UpdateUser(User user) => 
        FileInteractor
            .WriteAsync(_filePath, GetUsers().Select(u => u.Id == user.Id ? user : u).ToJson())
            .Wait();

    public void DeleteUser(string id) => 
        FileInteractor.WriteAsync(_filePath, GetUsers().Where(user => user.Id != id).ToJson()).Wait();


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