using Microsoft.AspNetCore.Mvc;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly UserService _userService;
    private readonly SessionService _sessionService;

    public UserController(UserService userService, SessionService sessionService)
    {
        _userService = userService;
        _sessionService = sessionService;
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        try
        {
            var existingUser = _userService.GetUserByUsername(user.Username);
            if (existingUser != null) return Unauthorized("User already exists");
            var userWithExistingEmail = _userService.GetUserByEmail(user.Email);
            if (userWithExistingEmail != null) return Unauthorized("The email is already in use!");
            var hashedUser = _userService.CreateUser(user) ?? throw new ArgumentException("Unable to create user");
            var currentTimestamp = DateTime.Now;
            var session = _sessionService.CreateSession(
                new Session
                {
                    UserId = hashedUser.Id,
                    CreatedAt = currentTimestamp,
                    ExpiresAt = currentTimestamp.AddMinutes(30)
                }
            );
            var response = Response;
            response.Cookies.Append("sessionId", session.Id);
            Console.WriteLine($"Session: {response.Cookies}");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return RedirectToAction("Index", "Car");
    }

    [HttpPost("login")]
    public IActionResult Read(string username, string password)
    {
        try
        {
            var user = _userService.GetUserByUsername(username);
            if (user == null || !_userService.VerifyPassword(password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var currentTimestamp = DateTime.Now;
            var existingSession = _sessionService.GetUserSessions(user.Id)
                .FirstOrDefault(session => session.ExpiresAt > currentTimestamp);
            var session = existingSession ?? _sessionService.CreateSession(new Session
            {
                UserId = user.Id,
                CreatedAt = currentTimestamp,
                ExpiresAt = currentTimestamp.AddMinutes(30)
            });
            Response.Cookies.Append("sessionId", session.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(e.Message);
        }

        return RedirectToAction("Index", "Car");
    }

    [HttpPut("{id}")]
    public IActionResult Update(User user)
    {
        _userService.UpdateUser(user);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var user = _userService.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }

        _userService.DeleteUser(user.Id);
        return NoContent();
    }
    
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        var session = _sessionService.GetSession(Request.Cookies["sessionId"]);
        if (session == null) return Unauthorized("You are not logged in!");
        session.ExpiresAt = DateTime.Now;
        _sessionService.UpdateSession(session);
        return RedirectToAction("Index", "Home");
    }
}