using Microsoft.AspNetCore.Mvc;

namespace RetroCarsWebApp.Controllers;

public class StoreController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}