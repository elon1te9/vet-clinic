using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VetClinic.Client;
using VetClinic.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7096") });
builder.Services.AddScoped<ApiRequestService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PetApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<AppointmentApiService>();
builder.Services.AddScoped<ClinicServiceApiService>();

var host = builder.Build();
var authService = host.Services.GetRequiredService<AuthService>();
await authService.RestoreUserAsync();

await host.RunAsync();
