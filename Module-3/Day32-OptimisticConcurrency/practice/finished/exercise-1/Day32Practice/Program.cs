using Microsoft.EntityFrameworkCore;
using Day32Practice.Components;
using Day32Practice.Data;
using Day32Practice.Services;
using Day32Practice.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day32Practice.db"));

builder.Services.AddDbContextFactory<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day32Practice.db"), ServiceLifetime.Scoped);

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IRestockService, RestockService>();
builder.Services.AddScoped<ConcurrencySimulator>();

var app = builder.Build();

// Recreate database to ensure RowVersion column exists after Practice 1
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
