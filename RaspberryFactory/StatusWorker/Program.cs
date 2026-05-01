using CommonFiles.Config;
using Logger;
using Logger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatusWorker;

var host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
})
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddDbContextFactory<LoggingContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("LogDb")));
        services.AddScoped<ILogService, LogService>();
        services.AddHostedService<LogBackgroundWorker>();
        services.Configure<MqttConfig>(configuration.GetSection("Mqtt")); 
        services.AddSingleton(sp => configuration.GetSection("Services").Get<List<ServicesConfig>>() ?? new List<ServicesConfig>());

        services.AddSingleton<Worker>();
    })
    .Build();
var worker = host.Services.GetRequiredService<Worker>();
var task1 = worker.RunServiceTelemetryAsync();
var task2 = worker.RunRaspberryTelemetryAsync();

await Task.WhenAll(task1, task2);
