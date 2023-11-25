using Microsoft.AspNetCore.Mvc;
using RetroCarsWebApp.Services;

namespace RetroCarsWebApp.Controllers
{
    [Route("api/[controller]")]
    public class StoreController : Controller
    {
        private readonly CarService _carService;
        private readonly UserService _userService;
        private readonly SessionService _sessionService;

        public StoreController(CarService carService, SessionService sessionService, UserService userService)
        {
            _carService = carService;
            _sessionService = sessionService;
            _userService = userService;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var availableCars = _carService.GetStoreCarsPaginated();
            return View(availableCars);
        }
        
        [HttpGet("Add")]
        public IActionResult Add()
        {
            var session = _sessionService.GetSession(Request.Cookies["sessionId"]);
            if (session == null) return Unauthorized("You are not logged in!");
            var userCars = _carService.GetUserCars(session.UserId).ToList();
            return View(userCars);
        }

        [HttpPost("Add")]
        public IActionResult Add(string carId)
        {
            var session = _sessionService.GetSession(Request.Cookies["sessionId"]);
            if (session == null) return Unauthorized("You are not logged in!");
            // _carService.AddCarToStore(carId);
            return RedirectToAction("Index");
        }
    }
}