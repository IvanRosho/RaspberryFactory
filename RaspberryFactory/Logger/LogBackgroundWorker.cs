using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class LogBackgroundWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public LogBackgroundWorker(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (LogQueue.TryDequeue(out var item))
                {
                    using var scope = _provider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ILogService>();

                    switch (item.Type)
                    {
                        case LogItemType.Log:
                            await service.LogAsync(
                                item.Log!.Text,
                                item.Log.Application,
                                item.Log.User,
                                item.Log.Comment);
                            break;

                        case LogItemType.LogError:
                            await service.LogErrorAsync(
                                item.LogError!.Text,
                                item.LogError.LineNumber,
                                Enum.Parse<LogLevel>(item.LogError.LogLevel),
                                item.LogError.Application,
                                item.LogError.File,
                                item.LogError.Function,
                                item.LogError.Comment);
                            break;
                    }
                }
                else
                {
                    await Task.Delay(50, stoppingToken);
                }
            }
        }
    }

}
