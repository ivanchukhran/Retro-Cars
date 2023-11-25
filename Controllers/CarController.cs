using Microsoft.AspNetCore.Mvc;
using RetroCarsWebApp.Models;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers;

[Route("api/[controller]")]
public class CarController : Controller
{
    private readonly CarService _carService;
    private readonly SessionService _sessionService;
    
    public CarController(CarService carService, SessionService sessionService)
    {
        _carService = carService;
        _sessionService = sessionService;
    }
    
    [HttpGet]
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var session = _sessionService.GetSession(Request.Cookies["sessionId"]);
        if (session == null) return Unauthorized("You are not logged in!");
        return View(_carService.GetUserCarsPaginated(session.UserId, page, pageSize));
    }

    [HttpGet("GetUserCars")]
    public IActionResult GetUserCars()
    {
        var session = _sessionService.GetSession(Request.Cookies["sessionId"]);
        if (session == null) return Unauthorized("You are not logged in!");
        return Ok(_carService.GetUserCarsPaginated(session.UserId, 1, 10));
    }
    
    [HttpGet("GetCars")]
    public IActionResult GetCars(int page = 1, int pageSize = 10)
    {
        return Ok(_carService.GetCars(page, pageSize));
    }
    
    [HttpPost("CreateCar")]
    public IActionResult CreateCar(Car car)
    {
        var session = _sessionService.GetSession(Request.Cookies["sessionId"]);
        if (session == null) return Unauthorized("You are not logged in!");
        car.OwnerId = session.UserId;
        _carService.CreateCar(car);
        return RedirectToAction("Index");
    }
    
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }
}