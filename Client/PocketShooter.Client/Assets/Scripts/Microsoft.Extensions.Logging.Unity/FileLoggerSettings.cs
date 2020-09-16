namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Settings.
    /// </summary>
    public class FileLoggerSettings
    {
        /// <summary>
        /// Gets the name of the log file.
        /// </summary>
        /// <value>
        /// The name of the log file.
        /// </value>
        public string LogFileName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerSettings"/> class.
        /// </summary>
        /// <param name="logFileName">Name of the log file.</param>
        public FileLoggerSettings(string logFileName) => LogFileName = logFileName;
    }
}