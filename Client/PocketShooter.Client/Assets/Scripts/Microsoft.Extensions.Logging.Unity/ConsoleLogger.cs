using System;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Writes to Unity Debug. Implement like .NET Debug provider.
    /// </summary>
    public class ConsoleLogger : ILogger, IDisposable
    {
        /// <summary>
        /// Avoid recreation and ensure single thread.
        /// </summary>
        private static StringBuilder logBuilder = new StringBuilder();

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">name.</exception>
        public ConsoleLogger(string name) =>
            Name = name ?? throw new ArgumentNullException(nameof(name));

        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => this;

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            lock (logBuilder)
            {
                logBuilder.Clear();

                var message = formatter(state, exception);

                // TODO: do all via append to avoid GC
                logBuilder.Append($"[{Name}] ({eventId}) {message}");

                if (!string.IsNullOrEmpty(message))
                {
                    logBuilder.AppendLine();
                }

                if (exception != null)
                {
                    logBuilder.AppendLine(exception.ToString());
                }

                string output = logBuilder.ToString();

                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        UnityEngine.Debug.Log(output);
                        break;
                    case LogLevel.Warning:
                        UnityEngine.Debug.LogWarning(output);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        UnityEngine.Debug.LogError(output);
                        break;
                    case LogLevel.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
