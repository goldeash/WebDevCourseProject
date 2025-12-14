using Microsoft.EntityFrameworkCore;
using WebDevCourseProject.Data;
using WebDevCourseProject.Models;

namespace WebDevCourseProject.Services;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _context;

    public TagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetUserTagsAsync(string userId)
    {
        return await _context.Tags
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteTagAsync(int id, string userId)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddTagToTaskAsync(int taskId, int tagId, string userId)
    {
        var task = await _context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.TodoList.UserId == userId);

        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);

        if (task != null && tag != null)
        {
            if (task.Tags == null)
            {
                task.Tags = new List<Tag>();
            }

            if (!task.Tags.Any(t => t.Id == tagId))
            {
                task.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId, string userId)
    {
        var task = await _context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.TodoList.UserId == userId);

        if (task != null && task.Tags != null)
        {
            var tagToRemove = task.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tagToRemove != null)
            {
                task.Tags.Remove(tagToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<List<TodoTask>> GetTasksByTagAsync(int tagId, string userId)
    {
        return await _context.Tasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Where(t => t.TodoList.UserId == userId &&
                       t.Tags != null &&
                       t.Tags.Any(tag => tag.Id == tagId))
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }
}