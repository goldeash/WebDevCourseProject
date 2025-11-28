using WebDevCourseProject.Models;

namespace WebDevCourseProject.Services;

public interface ITodoListService
{
    Task<List<TodoList>> GetUserTodoListsAsync(string userId);

    Task<TodoList?> GetTodoListByIdAsync(int id, string userId);

    Task CreateTodoListAsync(TodoList todoList);

    Task UpdateTodoListAsync(TodoList todoList);

    Task DeleteTodoListAsync(int id, string userId);
}