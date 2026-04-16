using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Logger
{
    public interface ILogService
    {
        Task LogErrorAsync(
            string message,
            int? CallerLineNumberAttribute,
            LogLevel logLevel = LogLevel.Debug,
            string Application = "", 
            string File = "", 
            string CallerMemberName = "", 
            string Comment = ""); 
        Task LogExceptionAsync(Exception ex,
            int? CallerLineNumberAttribute,
            string Application = "",
            string File = "",
            string CallerMemberName = "",
            string Comment = "");
        Task LogAsync(string Message, string Application = "", string User = "", string Comment = "");
    }

}
