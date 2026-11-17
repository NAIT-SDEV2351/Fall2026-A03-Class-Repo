using Microsoft.EntityFrameworkCore;
using Day30Practice.Components;
using Day30Practice.Data;
using Day30Practice.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SQLite — same provider as Days 26/27/29. No container, no LocalDB, no env var:
// just run it. Today's single SaveChangesAsync is one implicit transaction, which
// every provider supports.
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day30Practice.db"));

builder.Services.AddScoped<IOrderService, OrderService>();

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
