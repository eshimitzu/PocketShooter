using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Categorical static class to inherit from.
    /// All methods are conditional for zero overhead.
    /// Static singleton per category (not one, as one leads to bad practice of logging from low level classes and hard to split-share logger meta).
    /// See `Conditional` usage for zero cost logging in `DEBUG` builds.
    /// <see cref="Log" /> is for production and passing into low level entities (extendable by MS Logging).
    /// </summary>
    public abstract class MLog<T> where T : ICategoryLog
    {
        /*private*/
        protected MLog() => throw new System.NotSupportedException("https://github.com/dotnet/csharplang/issues/1631");

        static MLog()
        {
            var type = typeof(T);
            if (!type.Name.EndsWith(nameof(Log)))
            {
                throw new InvalidProgramException($"Categorical log type {type.Name} should end with {nameof(Log)}");
            }

            if (type.BaseType != typeof(MLog<T>))
            {
                // may be test for sealed or allow multi level inheritance, but still check
                throw new InvalidProgramException($"Categorical log type {typeof(MLog<T>)} should have recursive onto self");
            }

            Log = MLog.Factory.CreateLogger(Category);
        }

        /// <summary> Gets the log.</summary>
        public static ILogger Log { get; private set; }

        /// <summary>
        /// Debug only. Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        [Conditional("DEBUG")]
        public static void Trace(string message, params object[] args) => Log.LogTrace(message, args);

        /// <summary>
        /// Debug only. Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        [Conditional("DEBUG")]
        public static void Debug(string message, params object[] args) => Log.LogDebug(message, args);

        /// <summary>
        /// Debug only.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Information(string message, params object[] args) => Log.LogInformation(message, args);

        public static string Category => typeof(T).Name.Replace(nameof(Log), string.Empty);
    }

    /// <summary>
    /// Well known categories.
    /// </summary>
    public static class MLog
    {
        /// <summary>
        /// Gets default logging factory.
        /// </summary>
        public static ILoggerFactory Factory { get; private set; }

        static MLog() => Factory = new NullLoggerFactory();

#if !UNITY_5_3_OR_NEWER

        /// <summary>
        /// Setups the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void Setup(ILoggerFactory factory)
        {
            Factory = factory;
        }
#else

        private static LoggerFilterOptions loggerOptions;
        private static ILoggerConfiguration loggerConfig;

        /// <summary>
        /// Gets the logger config.
        /// </summary>
        /// <value>The logger config.</value>
        public static ILoggerConfiguration LoggerConfig => loggerConfig;

        /// <summary>
        /// Setups the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="options">The options.</param>
        /// <param name="config">The configuration.</param>
        public static void Setup(ILoggerFactory factory, LoggerFilterOptions options, ILoggerConfiguration config)
        {
            Factory = factory;
            loggerOptions = options;
            loggerConfig = config;
        }

        // https://github.com/dotnet/roslyn/issues/15983
        public static IReadOnlyList<string> GetCategories() =>
               AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .Where(x => x.GetInterfaces().Contains(typeof(ICategoryLog)))
                                .Select(x => x.Name.Replace("Log", string.Empty))
                                .ToList();
#endif
    }
}