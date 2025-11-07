using Microsoft.EntityFrameworkCore;
using Vetex.Models;

var builder = WebApplication.CreateBuilder(args);

// --- DbContext con la key correcta ---
builder.Services.AddDbContext<veterinariaContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("vetexDbConnection"))
);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
