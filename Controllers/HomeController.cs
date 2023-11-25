using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SessionService _sessionService;

    public HomeController(ILogger<HomeController> logger, SessionService sessionService)
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    public IActionResult Index()
    {
        var currentTimestamp = DateTime.Now;
        var lastActiveSession = _sessionService.GetLastActiveSession(currentTimestamp);
        if (lastActiveSession == null) return View();
        Response.Cookies.Append("sessionId", lastActiveSession.Id);
        return RedirectToAction("Index", "Car");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}