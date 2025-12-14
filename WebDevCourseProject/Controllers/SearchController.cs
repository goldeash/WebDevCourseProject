using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevCourseProject.Services;

namespace WebDevCourseProject.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly ITaskService _taskService;
    private readonly UserManager<IdentityUser> _userManager;

    public SearchController(ITaskService taskService, UserManager<IdentityUser> userManager)
    {
        _taskService = taskService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ModelState.AddModelError("", "Please enter search text");
            return View();
        }

        var userId = _userManager.GetUserId(User);
        var tasks = await _taskService.SearchTasksAsync(userId!, searchText);

        ViewBag.SearchText = searchText;
        return View("Results", tasks);
    }
}