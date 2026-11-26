using Microsoft.EntityFrameworkCore;
using Day34Practice.Components;
using Day34Practice.Data;
using Day34Practice.Services;
using Day34Practice.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Default: SQL Server LocalDB (Windows students). Override via DAY34_SQL_CONN
// env var to point at a remote/containerized SQL Server (used by the WSL demo
// recorder). Mirrors the Day34Demo wiring — LocalDB is Windows-only and is not
// reachable from WSL, so without this override the project cannot run there.
var sqlConn = Environment.GetEnvironmentVariable("DAY34_SQL_CONN")
    ?? @"Server=(localdb)\mssqllocaldb;Database=Day34Practice;Trusted_Connection=true";

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(sqlConn));

builder.Services.AddDbContextFactory<StoreDbContext>(options =>
    options.UseSqlServer(sqlConn), ServiceLifetime.Scoped);

builder.Services.AddScoped<ReservationService>();
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
