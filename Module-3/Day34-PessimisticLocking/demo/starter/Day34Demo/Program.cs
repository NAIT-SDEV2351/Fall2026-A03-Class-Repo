using Microsoft.EntityFrameworkCore;
using Day34Demo.Components;
using Day34Demo.Data;
using Day34Demo.Services;
using Day34Demo.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Default: SQL Server LocalDB (Windows instructors). Override via DAY34_SQL_CONN
// env var to point at a remote/containerized SQL Server (used by the WSL demo recorder).
var sqlConn = Environment.GetEnvironmentVariable("DAY34_SQL_CONN")
    ?? @"Server=(localdb)\mssqllocaldb;Database=Day34Demo;Trusted_Connection=true";

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(sqlConn));

builder.Services.AddDbContextFactory<StoreDbContext>(options =>
    options.UseSqlServer(sqlConn), ServiceLifetime.Scoped);

builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<ConcurrentRequestSimulator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
