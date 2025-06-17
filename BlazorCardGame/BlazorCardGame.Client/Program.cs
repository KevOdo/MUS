using BlazorCardGame.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register PlayerService as a scoped service
builder.Services.AddScoped<PlayerService>();

await builder.Build().RunAsync();
