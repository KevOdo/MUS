using BlazorCardGame.Components;
using BlazorCardGame.Shared;

var builder = WebApplication.CreateBuilder(args);

// Set custom URL/port for all network interfaces
builder.WebHost.UseUrls("http://0.0.0.0:5050");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents(); // Add this for WASM render mode support

builder.Services.AddScoped<PlayerService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery(); // Required for Blazor interactive endpoints

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.MapHub<GameHub>("/gamehub"); // SignalR hub mapping

app.Run();
