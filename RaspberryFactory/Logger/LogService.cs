using Logger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Logger
{
    public class LogService : ILogService
    {
        private readonly IDbContextFactory<LoggingContext> _factory;

        public LogService(IDbContextFactory<LoggingContext> factory) {
            _factory = factory;
        }

        public async Task LogAsync(string message, string application = "", string user = "", string comment = "")
        {
            await using var db = await _factory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(application)) application = Assembly.GetExecutingAssembly().GetName().Name ?? "";
            var entry = new Log
            {
                TimeStamp = DateTime.Now,
                Application = application,
                Text = message,
                User    = user,
                Comment = comment
            };

            await db.Log.AddAsync(entry);
            await db.SaveChangesAsync();
        }

        public async Task LogErrorAsync(string message, [CallerLineNumber]int callerLineNumber = 0, LogLevel logLevel = LogLevel.Debug, string application = "", [CallerFilePath]string file = "", [CallerMemberName]string callerMemberName = "", string comment = "")
        {
            await using var db = await _factory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(application)) application = Assembly.GetExecutingAssembly().GetName().Name ?? "";
            var entry = new LogError {
                TimeStamp = DateTime.Now,
                LogLevel = logLevel.ToString(),
                Application = application,
                File = file,
                Function = callerMemberName,
                Text = message,
                Comment = comment,
                LineNumber = callerLineNumber
            };

            await db.LogError.AddAsync(entry);
            await db.SaveChangesAsync();
        }

        public async Task LogExceptionAsync(Exception ex, [CallerLineNumber]int callerLineNumber = 0, string application = "", [CallerFilePath] string file = "", [CallerMemberName] string callerMemberName = "",  string comment = "")
        {
            await using var db = await _factory.CreateDbContextAsync();
            if (string.IsNullOrWhiteSpace(application)) application = Assembly.GetExecutingAssembly().GetName().Name ?? "";
            var entry = new LogError {
                TimeStamp = DateTime.Now,
                LogLevel = LogLevel.Error.ToString(),
                Application = application,
                File = file,
                Function = callerMemberName,
                Text = ex.ToString(),
                StackTrace = ex.StackTrace,
                Comment = comment,
                LineNumber = callerLineNumber
            };

            await db.LogError.AddAsync(entry);
            await db.SaveChangesAsync();
        }
    }

}
