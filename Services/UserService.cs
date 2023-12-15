using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NuGet.Protocol;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class UserService
{
    private readonly string _filePath;

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

    public User? GetUser(string id)
    {
        return GetUsers().FirstOrDefault(user => user.Id == id);
    }

    public User? GetUserByUsernameAndPassword(string username, string password)
    {
        return GetUsers().FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public User? GetUserByEmail(string userEmail)
    {
        return GetUsers().FirstOrDefault(u => u.Email == userEmail);
    }

    public User? GetUserByUsername(string username)
    {
        return GetUsers().FirstOrDefault(u => u.Username == username);
    }

    public void UpdateUser(User user)
    {
        FileInteractor
            .WriteAsync(_filePath, GetUsers().Select(u => u.Id == user.Id ? user : u).ToJson())
            .Wait();
    }

    public void DeleteUser(string id)
    {
        FileInteractor.WriteAsync(_filePath, GetUsers().Where(user => user.Id != id).ToJson()).Wait();
    }


    public string HashPassword(string password)
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA1,
                10000,
                256 / 8)
        );
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];
        var hashed = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA1,
                10000,
                256 / 8)
        );
        return hash == hashed;
    }
}