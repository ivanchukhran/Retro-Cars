using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

[Route("api/[controller]")]
public class StoreController : Controller
{
    private readonly CarService _carService;

    public StoreController(CarService carService)
    {
        _carService = carService;
    }

    [HttpGet]
    public IActionResult Index(int page = 1, int pageSize = 5)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");
        var cars = _carService
            .GetCars()
            .Where(c => c.IsAvailableOnStore && !c.OwnerId.Equals(userId))
            .ToList();
        return View(PaginatedList<Car>.Create(cars,page, pageSize));
    }

    [HttpPost("SellCar")]
    public async Task<IActionResult> SellCar()
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");

        string requestBody;
        using (var reader = new StreamReader(Request.Body))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        
        string carId = null;
        var parameters = requestBody.Split("&");
        if (parameters.Length > 0)
        {
            var keyValue = parameters[0].Split("=");
            if (keyValue.Length > 1)
            {
                carId = keyValue[1];
                // rest of your code
            }
            else
            {
                // handle the case where "=" is not present in the first parameter
            }
        }
        else
        {
            // handle the case where "&" is not present in the requestBody
        }
        if (carId == null) return BadRequest("Car ID is null!");
        var car = _carService.GetCar(carId);
        if (car == null) return NotFound("Car not found!");
        car.IsAvailableOnStore = true;
        _carService.UpdateCar(car);
        return RedirectToAction("Index");
}
    
    [HttpGet("RedirectToStoreMenu")]
    public IActionResult SellForm(int page = 1, int pageSize = 5)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? HttpContext.Session.GetString("userId") : null;
        if (userId == null) return Unauthorized("You are not logged in!");
        var cars = _carService
            .GetCars()
            .Where(c => c.OwnerId.Equals(userId) && !c.IsAvailableOnStore)
            .ToList();
        return View(PaginatedList<Car>.Create(cars, page, pageSize));
    }

    [HttpPost("RemoveFromStore")]
    public async Task<IActionResult> RemoveFromStore()
    {
        string requestBody;
        using (var reader = new StreamReader(Request.Body))
        {
            requestBody = await reader.ReadToEndAsync();
        }

        var carId = requestBody.Replace("\"", "");
        if (carId == null) return BadRequest("Car ID is null!");
        var car = _carService.GetCar(carId);
        if (car == null)
        {
            return BadRequest("The car does not exists!");
        }

        car.IsAvailableOnStore = false;
        _carService.UpdateCar(car);
        return Json(new { success = true });
}

    // GET
    public IActionResult Index()
    {
        return View();
    }
}