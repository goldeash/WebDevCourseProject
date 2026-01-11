using Microsoft.EntityFrameworkCore;
using WebDevCourseProject.Data;
using WebDevCourseProject.Models;

namespace WebDevCourseProject.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;

    public CommentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetTaskCommentsAsync(int taskId, string userId)
    {
        return await _context.Comments
            .Include(c => c.Task)
            .ThenInclude(t => t.TodoList)
            .Where(c => c.TaskId == taskId && c.Task.TodoList.UserId == userId)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(int commentId, string userId)
    {
        return await _context.Comments
            .Include(c => c.Task)
            .ThenInclude(t => t.TodoList)
            .FirstOrDefaultAsync(c => c.Id == commentId && c.Task.TodoList.UserId == userId);
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        comment.ModifiedDate = DateTime.UtcNow;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId, string userId)
    {
        var comment = await GetCommentByIdAsync(commentId, userId);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}