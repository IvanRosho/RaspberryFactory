using Logger;
using Logger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatusWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddDbContext<LoggingContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("LogDb")));
        services.AddScoped<ILogService, LogService>();
        services.AddHostedService<LogBackgroundWorker>();
        services.Configure<MqttConfig>(configuration.GetSection("Mqtt"));

        services.AddSingleton<Worker>();
    })
    .Build();

var worker = host.Services.GetRequiredService<Worker>();
await worker.RunAsync();
