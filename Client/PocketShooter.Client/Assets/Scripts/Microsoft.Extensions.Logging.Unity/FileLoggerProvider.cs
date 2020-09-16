using System.Collections.Concurrent;

namespace Microsoft.Extensions.Logging
{
    /// <inheritdoc/>
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, FileLogger> loggers = new ConcurrentDictionary<string, FileLogger>();

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public static string FullName => typeof(FileLoggerProvider).FullName;

        private FileLoggerSettings settings = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public FileLoggerProvider(FileLoggerSettings settings) => this.settings = settings;

        /// <inheritdoc/>
        public ILogger CreateLogger(string name) => loggers.GetOrAdd(name, x => new FileLogger(x, settings));

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}