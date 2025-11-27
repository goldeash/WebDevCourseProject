using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevCourseProject.Models;
using WebDevCourseProject.Services;

namespace WebDevCourseProject.Controllers;

[Authorize]
public class TodoListsController : Controller
{
    private readonly ITodoListService _todoListService;
    private readonly UserManager<IdentityUser> _userManager;

    public TodoListsController(ITodoListService todoListService, UserManager<IdentityUser> userManager)
    {
        _todoListService = todoListService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var todoLists = await _todoListService.GetUserTodoListsAsync(userId!);
        return View(todoLists);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoList todoList)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            todoList.UserId = userId!;

            await _todoListService.CreateTodoListAsync(todoList);
            return RedirectToAction(nameof(Index));
        }

        return View(todoList);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var todoList = await _todoListService.GetTodoListByIdAsync(id, userId!);

        if (todoList == null)
        {
            return NotFound();
        }

        return View(todoList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoList todoList)
    {
        if (id != todoList.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            var existingList = await _todoListService.GetTodoListByIdAsync(id, userId!);

            if (existingList == null)
            {
                return NotFound();
            }

            existingList.Title = todoList.Title;
            existingList.Description = todoList.Description;

            await _todoListService.UpdateTodoListAsync(existingList);
            return RedirectToAction(nameof(Index));
        }

        return View(todoList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        await _todoListService.DeleteTodoListAsync(id, userId!);
        return RedirectToAction(nameof(Index));
    }
}