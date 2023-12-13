using Microsoft.AspNetCore.Mvc;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

public class StoreController : Controller
{
    private readonly CarService _carService;
    
    public StoreController( CarService carService)
    {
        _carService = carService;
    }
    
    [HttpGet]
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var userId = HttpContext.Session.Keys.Contains("userId") ? 
            HttpContext.Session.GetString("userId") : 
            null;
        if (userId == null) return Unauthorized("You are not logged in!");
        // var userCars = _carService.GetUserCarsPaginated(userId, 1, 10);
        var cars = _carService
            .GetCars()
            .Where(c => !(c.OwnerId.Equals(userId)))
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToList();
        return View(new PaginatedList<Car>(cars, cars.Count, page, pageSize));
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }
}