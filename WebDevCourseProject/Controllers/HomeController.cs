using Microsoft.AspNetCore.Mvc;

namespace WebDevCourseProject.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
