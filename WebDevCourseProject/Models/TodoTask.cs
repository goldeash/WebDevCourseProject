using System.ComponentModel.DataAnnotations;

namespace WebDevCourseProject.Models;

public class TodoTask
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(1);

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Not Started";

    public int TodoListId { get; set; }

    public TodoList TodoList { get; set; } = null!;

    public string AssignedUserId { get; set; } = string.Empty;

    public bool IsOverdue => DueDate < DateTime.UtcNow && Status != "Completed";
}