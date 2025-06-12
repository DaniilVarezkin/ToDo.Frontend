using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ToDo.Frontend;
using ToDo.Frontend.Services.Auth;
using ToDo.Frontend.Services.TaskItems;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddScoped(provider =>
{
    var handler = provider.GetRequiredService<AuthHeaderHandler>();
    handler.InnerHandler = new HttpClientHandler();
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7011/")
    };
    return client;
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskItemsService, TaskItemsService>();


builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
