using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    public class LoggerFilterOptionsMonitor : IOptionsMonitor<LoggerFilterOptions>
    {
        private LoggerConfiguration configuration;

        private event Action<LoggerFilterOptions, string> ChangeListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFilterOptionsMonitor"/> class.
        /// </summary>
        /// <param name="currentValue">LoggerFilterOptions.</param>
        /// <param name="config">LoggerConfiguration.</param>
        public LoggerFilterOptionsMonitor(LoggerFilterOptions currentValue, LoggerConfiguration config)
        {
            CurrentValue = currentValue;
            configuration = config;

            ApplyFilters();
            config.OnChanged += Config_OnChanged;
        }

        /// <inheritdoc/>
        public LoggerFilterOptions CurrentValue { get; }

        /// <inheritdoc/>
        public IDisposable OnChange(Action<LoggerFilterOptions, string> listener)
        {
            ChangeListener += listener;
            return null;
        }

        /// <inheritdoc/>
        public LoggerFilterOptions Get(string name) => CurrentValue;

        private void Config_OnChanged(ILoggerConfiguration obj)
        {
            ApplyFilters();
            ChangeListener?.Invoke(CurrentValue, null);
        }

        private void ApplyFilters()
        {
            CurrentValue.Rules.Clear();
            foreach (var filter in configuration.Filters)
            {
                CurrentValue.Rules.Add(new LoggerFilterRule(filter.ProviderName, filter.Category, filter.Level, null));
            }

            // FIXME: v.shimkovich implement filtering of unknown categories or something.
            CurrentValue.Rules.Add(new LoggerFilterRule("Microsoft.Extensions.Logging.ConsoleLoggerProvider", "Microsoft.AspNetCore.SignalR.Client.HubConnection", LogLevel.Warning, null));
            CurrentValue.Rules.Add(new LoggerFilterRule("Microsoft.Extensions.Logging.ConsoleLoggerProvider", "Microsoft.AspNetCore.Http.Connections.Client.Internal.WebSocketsTransport", LogLevel.Warning, null));
            CurrentValue.Rules.Add(new LoggerFilterRule("Microsoft.Extensions.Logging.ConsoleLoggerProvider", "Heyworks.PocketShooter.Meta.Communication.GameHubClient", LogLevel.Warning, null));
            CurrentValue.Rules.Add(new LoggerFilterRule("Microsoft.Extensions.Logging.ConsoleLoggerProvider", "Microsoft.AspNetCore.Http.Connections.Client.HttpConnection", LogLevel.Warning, null));
        }
    }
}