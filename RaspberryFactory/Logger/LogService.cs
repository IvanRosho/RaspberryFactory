using Logger.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Logger
{
    public class LogService : ILogService
    {
        private readonly LoggingContext _db;

        public LogService(LoggingContext db)
        {
            _db = db;
        }

        public async Task LogAsync(string message, string application = "", string user = "", string comment = "")
        {
            var entry = new Log
            {
                Application = application,
                Text = message,
                User    = user,
                Comment = comment
            };

            _db.Log.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task LogErrorAsync(string message, int? callerLineNumber, LogLevel logLevel = LogLevel.Debug, string application = "", string file = "", string callerMemberName = "", string comment = "")
        {
            var entry = new LogError
            {
                LogLevel = logLevel.ToString(),
                Application = application,
                File = file,
                Function = callerMemberName,
                Text = message,
                Comment = comment,
                LineNumber = callerLineNumber
            };

            _db.LogError.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task LogExceptionAsync(Exception ex, int? callerLineNumber, string application = "", string file = "", string callerMemberName = "",  string comment = "")
        {
            var entry = new LogError
            {
                LogLevel = LogLevel.Error.ToString(),
                Application = application,
                File = file,
                Function = callerMemberName,
                Text = ex.ToString(),
                StackTrace = ex.StackTrace,
                Comment = comment,
                LineNumber = callerLineNumber
            };

            _db.LogError.Add(entry);
            await _db.SaveChangesAsync();
        }
    }

}
