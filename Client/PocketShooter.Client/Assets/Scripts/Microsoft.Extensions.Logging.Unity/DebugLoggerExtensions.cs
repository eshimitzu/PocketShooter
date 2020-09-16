using System.Diagnostics;

namespace Microsoft.Extensions.Logging
{
    public static class DebugLoggerExtensions
    {
        [Conditional("DEBUG")]
        public static void Trace(this ILogger self, string message, params object[] args) => self.LogTrace(message, args);

        [Conditional("DEBUG")]
        public static void Debug(this ILogger self, string message, params object[] args) => self.LogDebug(message, args);
        
        [Conditional("DEBUG")]
        public static void Information(this ILogger self, string message, params object[] args) => self.LogInformation(message, args);
    }
}