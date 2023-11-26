using Microsoft.AspNetCore.Mvc;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        var existingUser = _userService.GetUserByUsername(user.Username);
        if (existingUser != null || existingUser?.Email == user.Email) return Unauthorized("User already exists");
        var hashedUser = _userService.CreateUser(user) ?? throw new ArgumentException("Unable to create user");
        HttpContext.Session.SetString("userId", hashedUser.Id);
        return RedirectToAction("Index", "Car");
        
    }
    
    [HttpPost("login")]
    public IActionResult Read(string username, string password)
    {
        var user = _userService.GetUserByUsername(username);
        if (user == null || !_userService.VerifyPassword(password, user.Password))
        {
            return Unauthorized("Invalid username or password");
        }
        HttpContext.Session.SetString("userId", user.Id);
        return RedirectToAction("Index", "Car");
    }
    
    //
    // [HttpPut("{id}")]
    // public IActionResult Update(User user)
    // {
    //     _userService.UpdateUser(user);
    //     return Ok();
    // }
    //
    // [HttpDelete("{id}")]
    // public IActionResult Delete(string id)
    // {
    //     var user = _userService.GetUser(id);
    //     if (user == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     _userService.DeleteUser(user.Id);
    //     return NoContent();
    // }
    //
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("userId");
        return RedirectToAction("Index", "Home");
    }
}