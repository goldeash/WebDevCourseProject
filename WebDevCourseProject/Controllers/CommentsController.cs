using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevCourseProject.Models;
using WebDevCourseProject.Services;

namespace WebDevCourseProject.Controllers;

[Authorize]
public class CommentsController : Controller
{
    private readonly ICommentService _commentService;
    private readonly UserManager<IdentityUser> _userManager;

    public CommentsController(ICommentService commentService, UserManager<IdentityUser> userManager)
    {
        _commentService = commentService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int taskId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return RedirectToAction("Details", "Tasks", new { id = taskId });
        }

        var userId = _userManager.GetUserId(User);

        var comment = new Comment
        {
            Content = content.Trim(),
            UserId = userId!,
            TaskId = taskId,
            CreatedDate = DateTime.UtcNow
        };

        await _commentService.CreateCommentAsync(comment);
        return RedirectToAction("Details", "Tasks", new { id = taskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            var taskId = await GetTaskIdFromComment(id);
            return RedirectToAction("Details", "Tasks", new { id = taskId });
        }

        var userId = _userManager.GetUserId(User);
        var comment = await _commentService.GetCommentByIdAsync(id, userId!);

        if (comment == null)
        {
            return NotFound();
        }

        comment.Content = content.Trim();
        comment.ModifiedDate = DateTime.UtcNow;

        await _commentService.UpdateCommentAsync(comment);
        return RedirectToAction("Details", "Tasks", new { id = comment.TaskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        var comment = await _commentService.GetCommentByIdAsync(id, userId!);

        if (comment == null)
        {
            return NotFound();
        }

        var taskId = comment.TaskId;
        await _commentService.DeleteCommentAsync(id, userId!);

        return RedirectToAction("Details", "Tasks", new { id = taskId });
    }

    private async Task<int> GetTaskIdFromComment(int commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId, _userManager.GetUserId(User)!);
        return comment?.TaskId ?? 0;
    }
}