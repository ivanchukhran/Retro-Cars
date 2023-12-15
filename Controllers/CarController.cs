using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RetroCarsWebApp.Classification;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

[Route("api/[controller]")]
public class CarController : Controller
{
    private readonly CarService _carService;
    private readonly PredictionService _predictionService;

    public CarController(CarService carService, PredictionService predictionService)
    {
        _carService = carService;
        _predictionService = predictionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");
        var userCars = _carService.GetUserCars(userId).ToList();
        return View(PaginatedList<Car>.Create(userCars, page, pageSize));
    }

    [HttpGet("GetCars")]
    public IActionResult GetCars(int page = 1, int pageSize = 5)
    {
        if (page < 1) page = 1;
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");
        var userCars = _carService.GetUserCars(userId).ToList();
        return Ok(PaginatedList<Car>.Create(userCars, page, pageSize));
    }

    [HttpPost("CreateCar")]
    public IActionResult CreateCar(Car car)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");
        Console.WriteLine($"Car is: {car}");
        car.OwnerId = userId;
        _carService.CreateCar(car);
        return RedirectToAction("Index");
    }

    [HttpPost("PredictCarClass")]
    public async Task<IActionResult> PredictCarClass()
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");
        string requestBody;
        using (var reader = new StreamReader(Request.Body))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        
        var car = JsonConvert.DeserializeObject<Car>(requestBody);
        Classification.Classification.Car predictionCar = new Classification.Classification.Car
        {
            Buying = car.BuyingPrice,
            Maint = car.MaintenanceCost,
            Doors = car.NumberOfDoors.ToString(),
            Persons = car.NumberOfSeats.ToString(),
            LugBoot = car.LuggageCapacity,
            Safety = car.SafetyRating
        };

        var carClass = _predictionService.PredictCarClass(predictionCar);
        return Json( new { carClass });
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("BuyCar")]
    public async Task<IActionResult> ChangeOwner()
    {
        var newOwnerId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (newOwnerId == null) return Unauthorized("You are not logged in!");

        string requestBody;
        using (var reader = new StreamReader(Request.Body))
        {
            requestBody = await reader.ReadToEndAsync();
        }

        var carId = JsonConvert.DeserializeObject<string>(requestBody);
        if (carId == null) return BadRequest("Car ID is null!");
        var car = _carService.GetCar(carId);
        if (car == null) return NotFound("Car not found");
        car.OwnerId = newOwnerId;
        car.IsAvailableOnStore = false;
        _carService.UpdateCar(car);
        return Json(new { success = true });
    }
}