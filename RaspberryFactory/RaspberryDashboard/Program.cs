using CommonFiles;
using CommonFiles.Config;
using CommonFiles.TelemetryDTO;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using RaspberryDashboard.Repository;
using System.Globalization;
using System.Text.Json;

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
            var mqttConfig = new MqttConfig();
            builder.Configuration.GetSection("Mqtt").Bind(mqttConfig); 
            builder.Services.AddSingleton(mqttConfig); 
            builder.Services.AddSingleton<TelemetryState>();

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
