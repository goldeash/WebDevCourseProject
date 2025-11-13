var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{ 
    app.UseDeveloperExceptionPage();
}

app.MapStaticAssets();

app.MapDefaultControllerRoute();

app.Run();
