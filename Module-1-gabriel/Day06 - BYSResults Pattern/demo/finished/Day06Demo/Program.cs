using Fluxor;
using Day06Demo.Services;
using Day06Demo.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ScanAssemblies finds and registers every action, reducer, effect, and feature in this
// assembly by reflection — that's why none of the store classes are wired up by hand.
builder.Services.AddFluxor(options =>
    options.ScanAssemblies(typeof(Program).Assembly));

// Service for the dashboard demonstration
builder.Services.AddScoped<IDashboardService, MockDashboardService>();

var app = builder.Build();

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
