using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebDevCourseProject.Models;

namespace WebDevCourseProject.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoList> TodoLists { get; set; }
    public DbSet<TodoTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TodoList>()
            .HasMany(tl => tl.Tasks)
            .WithOne(t => t.TodoList)
            .HasForeignKey(t => t.TodoListId);
    }
}