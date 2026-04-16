# How To Use

```
services.AddDbContext<LoggingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LogDb")));

services.AddScoped<ILogService, LogService>();
services.AddHostedService<LogBackgroundWorker>();
```