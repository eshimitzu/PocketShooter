using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Photon.Logging
{
    internal sealed class ExitGamesLoggerFactory : ExitGames.Logging.ILoggerFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public ExitGamesLoggerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public ExitGames.Logging.ILogger CreateLogger(string name)
        {
            var logger = loggerFactory.CreateLogger(name);

            return new ExitGamesLogger(logger, name);
        }
    }
}
