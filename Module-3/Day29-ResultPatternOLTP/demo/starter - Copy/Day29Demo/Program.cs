using Microsoft.EntityFrameworkCore;
using Day29Demo.Components;
using Day29Demo.Data;
using Day29Demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SQLite — same provider as Days 26/27. The explicit BeginTransactionAsync
// boundary this day uses works on any provider. No container, no LocalDB,
// no env var: just run it.
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day29Store.db"));

builder.Services.AddScoped<ICheckoutService, CheckoutService>();

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
