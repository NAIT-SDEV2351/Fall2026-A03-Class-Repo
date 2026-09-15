using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Day05Demo.Components;
using Day05Demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ScanAssemblies finds and registers every action, reducer, effect, and feature in this
// assembly by reflection — that's why none of the store classes are wired up by hand.
// UseReduxDevTools streams each dispatched action to the browser's Redux DevTools so you
// can watch the LoadProducts → Success/Failure sequence live.
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools();
});

// Registering against the INTERFACE is what lets ProductEffects inject IProductService and
// stay decoupled from the concrete mock. Swap this one line for a real service and nothing
// in the store changes. Scoped = one instance per connection (the Blazor Server default).
builder.Services.AddScoped<IProductService, MockProductService>();

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
