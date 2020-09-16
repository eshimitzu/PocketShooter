using System;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Photon.Logging
{
    internal sealed class ExitGamesLogger : ExitGames.Logging.ILogger
    {
        private readonly ILogger logger;

        public ExitGamesLogger(ILogger logger, string name)
        {
            this.logger = logger;
            this.Name = name;
        }

        public bool IsDebugEnabled => logger.IsEnabled(LogLevel.Debug);

        public bool IsErrorEnabled => logger.IsEnabled(LogLevel.Error);

        public bool IsFatalEnabled => logger.IsEnabled(LogLevel.Critical);

        public bool IsInfoEnabled => logger.IsEnabled(LogLevel.Information);

        public bool IsWarnEnabled => logger.IsEnabled(LogLevel.Warning);

        public string Name { get; }

        public void Debug(object message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Debug(object message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            DebugFormat(format, args);
        }

        public void Error(object message)
        {
            Log(LogLevel.Error, message);
        }

        public void Error(object message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            Log(LogLevel.Critical, message);
        }

        public void Fatal(object message, Exception exception)
        {
            Log(LogLevel.Critical, message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Log(LogLevel.Critical, format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            FatalFormat(format, args);
        }

        public void Info(object message)
        {
            Log(LogLevel.Information, message);
        }

        public void Info(object message, Exception exception)
        {
            Log(LogLevel.Information, message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Log(LogLevel.Information, format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            Log(LogLevel.Warning, message);
        }

        public void Warn(object message, Exception exception)
        {
            Log(LogLevel.Warning, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Log(LogLevel.Warning, format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            WarnFormat(format, args);
        }

        private void Log(LogLevel logLevel, object message)
        {
            logger.Log(logLevel, message.ToString());
        }

        private void Log(LogLevel logLevel, object message, Exception exception)
        {
            logger.Log(logLevel, exception, message.ToString());
        }

        private void Log(LogLevel logLevel, string format, params object[] args)
        {
            logger.Log(logLevel, format, args);
        }
    }
}
