using System.Net;
using Microsoft.AspNetCore.Mvc;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

[Route("api/[controller]")]
public class CarController : Controller
{
    private readonly CarService _carService;
    
    public CarController(CarService carService)
    {
        _carService = carService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? 
            HttpContext.Session.GetString("userId") : 
            null;
        if (userId == null) return Unauthorized("You are not logged in!");
        var userCars = _carService.GetUserCars(userId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View(new PaginatedList<Car>(userCars, userCars.Count, page, pageSize));
    }
        
    // [HttpGet("GetUserCars")]
    // public IActionResult GetUserCars()
    // {
    //     var userId = HttpContext.Session.Keys.Contains("userId") ? 
    //         HttpContext.Session.GetString("userId") : 
    //         null;
    //     return userId == null ? 
    //         Unauthorized("You are not logged in!") : 
    //         Ok(_carService.GetUserCars(userId));
    // }

    [HttpGet("GetCars")]
    public IActionResult GetCars(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        var userId = HttpContext.Session.Keys.Contains("userId") ? 
            HttpContext.Session.GetString("userId") : 
            null;
        return userId == null ? 
            Unauthorized("You are not logged in!") : 
            Ok(_carService.GetUserCars(userId).Skip((page - 1) * pageSize).Take(pageSize));
    }

    [HttpPost("CreateCar")]
    public IActionResult CreateCar(Car car)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? 
            HttpContext.Session.GetString("userId") : 
            null;
        if (userId == null) return Unauthorized("You are not logged in!");
        car.OwnerId = userId;
        _carService.CreateCar(car);
        return RedirectToAction("Index");
    }
    
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }
}