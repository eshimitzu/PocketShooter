using System;

namespace Heyworks.PocketShooter.Realtime
{
    // TODO: v.shimkovich split config into common/client-only/transferable constants  (as RealtimeConfiguration)
    public static class Constants
    {
        public const int BufferSize = 128;

        public const int TickIntervalMs = 30;

        private const float MaximumClientOffsetMs = 2_000;

        // prevent client to send commands to far into future
        public const int AcceptedClientTickOffset = (int)(MaximumClientOffsetMs / TickIntervalMs) + 1;

        public const byte CommandsResendCountMax = 4;

        public const byte MaxZonesCount = 10;

        public static int ToTicks(int intervalMs) => (int)Math.Ceiling((double)intervalMs / TickIntervalMs);

        public static int ToMilliseconds(int ticks) => ticks * TickIntervalMs;

        public static float ToSeconds(int ticks) => ToMilliseconds(ticks) / 1000f;

        #region Client-only constants

        public static readonly string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

        #endregion
    }
}
