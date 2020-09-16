using System.Collections.Concurrent;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Creates named loggers.
    /// </summary>
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> loggers = new ConcurrentDictionary<string, ConsoleLogger>();

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public static string FullName => typeof(ConsoleLoggerProvider).FullName;

        /// <inheritdoc/>
        public ILogger CreateLogger(string name) =>
            loggers.GetOrAdd(name, x => new ConsoleLogger(x));

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}