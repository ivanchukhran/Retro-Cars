using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (!HttpContext.Session.Keys.Contains("userId"))
        {
            return View();
        }
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