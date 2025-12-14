using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebDevCourseProject.Models;
using WebDevCourseProject.Services;

namespace WebDevCourseProject.Controllers;

[Authorize]
public class TagsController : Controller
{
    private readonly ITagService _tagService;
    private readonly UserManager<IdentityUser> _userManager;

    public TagsController(ITagService tagService, UserManager<IdentityUser> userManager)
    {
        _tagService = tagService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var tags = await _tagService.GetUserTagsAsync(userId);
        return View(tags);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var userId = _userManager.GetUserId(User);
            var tag = new Tag
            {
                Name = name.Trim(),
                UserId = userId!
            };

            await _tagService.CreateTagAsync(tag);
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Tag name is required");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        await _tagService.DeleteTagAsync(id, userId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> TasksByTag(int id)
    {
        var userId = _userManager.GetUserId(User);
        var tasks = await _tagService.GetTasksByTagAsync(id, userId);

        var tags = await _tagService.GetUserTagsAsync(userId);
        var tag = tags.FirstOrDefault(t => t.Id == id);

        ViewBag.TagName = tag?.Name ?? "Tag";
        return View(tasks);
    }
}