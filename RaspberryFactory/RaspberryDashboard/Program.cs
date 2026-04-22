using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;
using RaspberryDashboard.Config;
using RaspberryDashboard.Logic;
using RaspberryDashboard.Repository;
using System.Globalization;

namespace RaspberryDashboard {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");


            builder.Services.AddMudServices();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<NetworkInfoService>();
            builder.Services.AddMudServices();
            builder.Services.AddScoped<LanguageService>();

            var host = builder.Build();
            var net = host.Services.GetRequiredService<NetworkInfoService>();
            await net.InitializeAsync();
            //Default-Culture setzen
            CultureInfo culture;
            var js = host.Services.GetRequiredService<IJSRuntime>();
            var result = await js.InvokeAsync<string>("blazorCulture.get");

            if (result != null) {
                culture = new CultureInfo(result);
            }
            else {
                culture = new CultureInfo("de-DE");
                await js.InvokeVoidAsync("blazorCulture.set", "de-DE");
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;


            await host.RunAsync();
        }
    }
}
