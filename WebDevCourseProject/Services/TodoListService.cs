using Microsoft.EntityFrameworkCore;
using WebDevCourseProject.Data;
using WebDevCourseProject.Models;

namespace WebDevCourseProject.Services;

public class TodoListService : ITodoListService
{
    private readonly ApplicationDbContext _context;

    public TodoListService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TodoList>> GetUserTodoListsAsync(string userId)
    {
        return await _context.TodoLists
            .Include(tl => tl.Tasks)
            .Where(tl => tl.UserId == userId)
            .OrderBy(tl => tl.Title)
            .ToListAsync();
    }

    public async Task<TodoList?> GetTodoListByIdAsync(int id, string userId)
    {
        return await _context.TodoLists
            .Include(tl => tl.Tasks)
            .FirstOrDefaultAsync(tl => tl.Id == id && tl.UserId == userId);
    }

    public async Task CreateTodoListAsync(TodoList todoList)
    {
        _context.TodoLists.Add(todoList);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTodoListAsync(TodoList todoList)
    {
        _context.TodoLists.Update(todoList);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTodoListAsync(int id, string userId)
    {
        var todoList = await GetTodoListByIdAsync(id, userId);
        if (todoList != null)
        {
            _context.TodoLists.Remove(todoList);
            await _context.SaveChangesAsync();
        }
    }
}