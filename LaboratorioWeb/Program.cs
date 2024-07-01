using LaboratorioWeb.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));

builder.WebHost.UseKestrel(so =>
{
    so.Limits.MaxConcurrentConnections = 100;
    so.Limits.MaxConcurrentUpgradedConnections = 100;
    so.Limits.MaxRequestBodySize = 52428800;
});
var app = builder.Build();

ThreadPool.SetMinThreads(10, 10);
ThreadPool.SetMaxThreads(100, 100);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
