using Microsoft.EntityFrameworkCore;
using Vetex.Models;

var builder = WebApplication.CreateBuilder(args);

// --- DbContext con la key correcta ---
builder.Services.AddDbContext<veterinariaContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("vetexDbConnection"))
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
