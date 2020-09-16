using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerTimeProvider : IDateTimeProvider
    {
        private readonly TimeSpan serverTimeDelta;
        private static readonly TimeSpan NetworkMarginDelay = TimeSpan.FromSeconds(0.5);

        public ServerTimeProvider(DateTime serverTimeUtcNow)
        {
            serverTimeDelta = serverTimeUtcNow - DateTime.UtcNow;
        }

        public DateTime UtcNow => DateTime.UtcNow + serverTimeDelta - NetworkMarginDelay;
    }
}
