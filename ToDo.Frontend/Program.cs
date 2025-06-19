using Blazored.LocalStorage;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Syncfusion.Blazor;
using System.Reflection;
using ToDo.Frontend;
using ToDo.Frontend.Services.Auth;
using ToDo.Frontend.Services.Statistic;
using ToDo.Frontend.Services.TaskItems;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthHeaderHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.Configuration["api"])
    };
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskItemsService, TaskItemsService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
