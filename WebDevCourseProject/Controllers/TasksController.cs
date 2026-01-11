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
    private readonly ITagService _tagService;
    private readonly ICommentService _commentService;
    private readonly UserManager<IdentityUser> _userManager;

    public TasksController(ITaskService taskService, ITodoListService todoListService,
                          UserManager<IdentityUser> userManager, ITagService tagService,
                          ICommentService commentService)
    {
        _taskService = taskService;
        _todoListService = todoListService;
        _userManager = userManager;
        _tagService = tagService;
        _commentService = commentService;
    }

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

    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _taskService.GetTaskByIdAsync(id, userId!);

        if (task == null)
        {
            return NotFound();
        }

        ViewBag.CommentService = _commentService;
        ViewBag.CurrentUserId = userId;

        return View(task);
    }

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
            AssignedUserId = userId!,
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = "Not Started"
        };

        var allTags = await _tagService.GetUserTagsAsync(userId!);
        ViewBag.AllTags = allTags;

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoTask task, List<int> SelectedTagIds)
    {
        var userId = _userManager.GetUserId(User);
        var todoList = await _todoListService.GetTodoListByIdAsync(task.TodoListId, userId!);

        if (todoList == null)
        {
            return NotFound();
        }

        task.AssignedUserId = userId!;
        task.CreatedDate = DateTime.UtcNow;

        ModelState.Remove("TodoList");
        ModelState.Remove("Tags");

        if (ModelState.IsValid)
        {
            await _taskService.CreateTaskAsync(task);

            if (SelectedTagIds != null)
            {
                foreach (var tagId in SelectedTagIds)
                {
                    await _tagService.AddTagToTaskAsync(task.Id, tagId, userId!);
                }
            }

            return RedirectToAction(nameof(Index), new { listId = task.TodoListId });
        }

        var allTags = await _tagService.GetUserTagsAsync(userId!);
        ViewBag.AllTags = allTags;

        return View(task);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var task = await _taskService.GetTaskByIdAsync(id, userId!);

        if (task == null)
        {
            return NotFound();
        }

        var allTags = await _tagService.GetUserTagsAsync(userId!);
        var taskTags = await _taskService.GetTaskTagsAsync(id, userId!);

        ViewBag.AllTags = allTags;
        ViewBag.TaskTags = taskTags.Select(t => t.Id).ToList();

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoTask task, List<int> SelectedTagIds)
    {
        if (id != task.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var existingTask = await _taskService.GetTaskByIdAsync(id, userId!);

        if (existingTask == null)
        {
            return NotFound();
        }

        ModelState.Remove("TodoList");
        ModelState.Remove("AssignedUserId");
        ModelState.Remove("CreatedDate");
        ModelState.Remove("Tags");

        if (ModelState.IsValid)
        {
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;

            await _taskService.UpdateTaskAsync(existingTask);

            var currentTags = await _taskService.GetTaskTagsAsync(id, userId!);
            var currentTagIds = currentTags.Select(t => t.Id).ToList();

            if (SelectedTagIds != null)
            {
                foreach (var tagId in SelectedTagIds)
                {
                    if (!currentTagIds.Contains(tagId))
                    {
                        await _tagService.AddTagToTaskAsync(id, tagId, userId!);
                    }
                }
            }

            foreach (var currentTagId in currentTagIds)
            {
                if (SelectedTagIds == null || !SelectedTagIds.Contains(currentTagId))
                {
                    await _tagService.RemoveTagFromTaskAsync(id, currentTagId, userId!);
                }
            }

            return RedirectToAction(nameof(Index), new { listId = existingTask.TodoListId });
        }

        var allTags = await _tagService.GetUserTagsAsync(userId!);
        var taskTags = await _taskService.GetTaskTagsAsync(id, userId!);

        ViewBag.AllTags = allTags;
        ViewBag.TaskTags = taskTags.Select(t => t.Id).ToList();

        return View(task);
    }

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