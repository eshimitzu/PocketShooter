using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Logs to file.
    /// </summary>
    public class FileLogger : ILogger
    {
        private static StringBuilder logBuilder = new StringBuilder(50);
        private readonly FileLoggerSettings settings;
        private readonly string directory;
        private readonly string path;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException">name.</exception>
        public FileLogger(string name, FileLoggerSettings settings)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            this.settings = settings;

            directory = Path.Combine(Application.persistentDataPath, "logs");
            path = Path.Combine(directory, settings.LogFileName);
        }

        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => state as IDisposable;

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

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                string message = formatter(state, exception);

                logBuilder
                    .Append(timestamp)
                    .Append(" [")
                    .Append(GetLogLevelString(logLevel))
                    .Append("] ")
                    .Append(message)
                    .Append(" [")
                    .Append(eventId)
                    .Append("]");

                if (exception != null)
                {
                    logBuilder.AppendLine(exception.ToString());
                }

                logBuilder.AppendLine();

                string output = logBuilder.ToString();

                WriteLogInstant(output);
            }
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "Trace";
                case LogLevel.Debug:
                    return "Debug";
                case LogLevel.Information:
                    return "Info";
                case LogLevel.Warning:
                    return "Warning";
                case LogLevel.Error:
                    return "Error";
                case LogLevel.Critical:
                    return "Critical";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private void WriteLogInstant(string text)
        {
            try
            {
                Directory.CreateDirectory(directory);
                File.AppendAllText(path, text);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}