using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Presentation.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
