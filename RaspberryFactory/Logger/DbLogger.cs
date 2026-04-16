using Logger.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class DbLogger
    {
        public static void Log(
            string message,
            string application = "",
            string user = "",
            string comment = "")
        {
            LogQueue.Enqueue(new LogQueueItem
            {
                Type = LogItemType.Log,
                Log = new Log
                {
                    Application = application,
                    Text = message,
                    User = user,
                    Comment = comment
                }
            });
        }

        public static void LogError(
            string message,
            LogLevel logLevel = LogLevel.Debug,
            string application = "",
            [CallerFilePath]string file = "",
            [CallerMemberName]string callerMemberName = "",
            [CallerLineNumber]int? callerLineNumber = null,
            string comment = "")
        {
            LogQueue.Enqueue(new LogQueueItem
            {
                Type = LogItemType.LogError,
                LogError = new LogError
                {
                    LogLevel = logLevel.ToString(),
                    Application = application,
                    File = file,
                    Function = callerMemberName,
                    Text = message,
                    Comment = comment,
                    LineNumber = callerLineNumber
                }
            });
        }

        public static void LogException(
            Exception ex,
            string application = "",
            [CallerFilePath] string file = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int? callerLineNumber = null,
            string comment = "")
        {
            LogQueue.Enqueue(new LogQueueItem
            {
                Type = LogItemType.LogError,
                LogError = new LogError
                {
                    LogLevel = LogLevel.Error.ToString(),
                    Application = application,
                    File = file,
                    Function = callerMemberName,
                    Text = ex.ToString(),
                    StackTrace = ex.StackTrace,
                    Comment = comment,
                    LineNumber = callerLineNumber
                }
            });
        }
    }

}
