using Microsoft.EntityFrameworkCore;
using Day27Demo.Components;
using Day27Demo.Data;
using Day27Demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlite("Data Source=Day27Store.db"));

builder.Services.AddScoped<CheckoutService>();

// Default payment gateway approves every charge. To demo the rollback path (a declined
// payment between the two saves), swap the line below for the FailingPaymentGateway:
//   builder.Services.AddScoped<IPaymentGateway, FailingPaymentGateway>();
builder.Services.AddScoped<IPaymentGateway, FailingPaymentGateway>();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
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
