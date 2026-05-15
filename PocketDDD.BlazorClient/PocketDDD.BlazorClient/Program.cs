using Blazored.LocalStorage;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PocketDDD.BlazorClient;
using PocketDDD.BlazorClient.Features.Sync.Services;
using PocketDDD.BlazorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["apiUrl"]!) });
builder.Services.AddMudServices();
builder.Services.AddFluxor(o =>
{
    o.ScanAssemblies(typeof(Program).Assembly);
    // Removing because there are problems insttiating this since .NET 7 -> if you want to fix then ensure that all unintitialised-type build warnings are fixed
    //    if (builder.HostEnvironment.IsDevelopment())
    //        o.UseReduxDevTools();
});
builder.Services.AddBlazoredLocalStorage();

if (builder.Configuration.GetValue<bool>("fakeBackend") == false)
    builder.Services.AddScoped<IPocketDDDApiService, PocketDDDApiService>();
else
    builder.Services.AddScoped<IPocketDDDApiService, FakePocketDDDApiService>();

builder.Services.AddScoped<LocalStorageContext>();
builder.Services.AddScoped<SyncService>();

await builder.Build().RunAsync();
