using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevCourseProject.Services;

namespace WebDevCourseProject.Controllers;

[Authorize]
public class AssignedTasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly UserManager<IdentityUser> _userManager;

    public AssignedTasksController(ITaskService taskService, UserManager<IdentityUser> userManager)
    {
        _taskService = taskService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string statusFilter = "active", string sortBy = "duedate")
    {
        var userId = _userManager.GetUserId(User);

        ViewData["StatusFilter"] = statusFilter;
        ViewData["SortBy"] = sortBy;

        var tasks = await _taskService.GetAssignedTasksFilteredAsync(userId!, statusFilter);
        tasks = sortBy switch
        {
            "title" => tasks.OrderBy(t => t.Title).ToList(),
            "createddate" => tasks.OrderByDescending(t => t.CreatedDate).ToList(),
            _ => tasks.OrderBy(t => t.DueDate).ToList()
        };

        return View(tasks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int taskId, string newStatus)
    {
        var userId = _userManager.GetUserId(User);
        await _taskService.UpdateTaskStatusAsync(taskId, userId!, newStatus);
        return RedirectToAction(nameof(Index));
    }
}