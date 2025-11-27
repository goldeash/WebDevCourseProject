using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevCourseProject.Models;
using WebDevCourseProject.Services;

namespace WebDevCourseProject.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ITodoListService _todoListService;
    private readonly UserManager<IdentityUser> _userManager;

    public TasksController(ITaskService taskService, ITodoListService todoListService, UserManager<IdentityUser> userManager)
    {
        _taskService = taskService;
        _todoListService = todoListService;
        _userManager = userManager;
    }

    // US05: View the list of tasks in a to-do list
    public async Task<IActionResult> Index(int listId)
    {
        var userId = _userManager.GetUserId(User);
        var todoList = await _todoListService.GetTodoListByIdAsync(listId, userId!);

        if (todoList == null)
        {
            return NotFound();
        }

        ViewData["TodoList"] = todoList;
        var tasks = await _taskService.GetTasksByListAsync(listId, userId!);
        return View(tasks);
    }

    // US06: View the task details page
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _taskService.GetTaskByIdAsync(id, userId!);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // US07: Add a new to-do task
    public async Task<IActionResult> Create(int listId)
    {
        var userId = _userManager.GetUserId(User);
        var todoList = await _todoListService.GetTodoListByIdAsync(listId, userId!);

        if (todoList == null)
        {
            return NotFound();
        }

        var task = new TodoTask
        {
            TodoListId = listId,
            AssignedUserId = userId!
        };

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoTask task)
    {
        var userId = _userManager.GetUserId(User);

        // Проверяем что TodoList существует и принадлежит пользователю
        var todoList = await _todoListService.GetTodoListByIdAsync(task.TodoListId, userId!);
        if (todoList == null)
        {
            return NotFound();
        }

        // Устанавливаем обязательные поля которые не приходят из формы
        task.AssignedUserId = userId!;
        task.CreatedDate = DateTime.UtcNow;

        // Очищаем ошибку валидации для TodoList, так как мы установили его вручную
        ModelState.Remove("TodoList");

        if (ModelState.IsValid)
        {
            await _taskService.CreateTaskAsync(task);
            return RedirectToAction(nameof(Index), new { listId = task.TodoListId });
        }

        // Если валидация не прошла, возвращаем форму с ошибками
        return View(task);
    }

    // US09: Edit a to-do task
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _taskService.GetTaskByIdAsync(id, userId!);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoTask task)
    {
        if (id != task.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        // Проверяем что задача существует и принадлежит пользователю
        var existingTask = await _taskService.GetTaskByIdAsync(id, userId!);
        if (existingTask == null)
        {
            return NotFound();
        }

        // Очищаем ошибку валидации для TodoList и других навигационных свойств
        ModelState.Remove("TodoList");
        ModelState.Remove("AssignedUserId");
        ModelState.Remove("CreatedDate");

        if (ModelState.IsValid)
        {
            // Обновляем только разрешенные поля
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;

            await _taskService.UpdateTaskAsync(existingTask);
            return RedirectToAction(nameof(Index), new { listId = existingTask.TodoListId });
        }

        // Если валидация не прошла, возвращаем форму с ошибками
        return View(task);
    }

    // US08: Delete a to-do task
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _taskService.GetTaskByIdAsync(id, userId!);

        if (task == null)
        {
            return NotFound();
        }

        var listId = task.TodoListId;
        await _taskService.DeleteTaskAsync(id, userId!);
        return RedirectToAction(nameof(Index), new { listId });
    }
}