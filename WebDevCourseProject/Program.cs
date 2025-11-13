using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebDevCourseProject.Data;
using WebDevCourseProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Добавляем Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем Identity с поддержкой Razor Pages
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Добавляем Razor Pages поддержку
builder.Services.AddRazorPages();

// Добавляем наш сервис
builder.Services.AddScoped<ITodoListService, TodoListService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Добавляем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();