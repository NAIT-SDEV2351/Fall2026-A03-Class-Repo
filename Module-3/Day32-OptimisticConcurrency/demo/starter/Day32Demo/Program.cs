using Microsoft.EntityFrameworkCore;
using Day32Demo.Components;
using Day32Demo.Data;
using Day32Demo.Services;
using Day32Demo.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day32Store.db"));

// IDbContextFactory for creating separate contexts (concurrency simulation)
builder.Services.AddDbContextFactory<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day32Store.db"), ServiceLifetime.Scoped);

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ConcurrencySimulator>();

var app = builder.Build();

// Recreate database to ensure RowVersion column exists
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
