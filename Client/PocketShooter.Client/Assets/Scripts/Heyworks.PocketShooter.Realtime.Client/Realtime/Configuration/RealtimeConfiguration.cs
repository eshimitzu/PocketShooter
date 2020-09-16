using System.Diagnostics.CodeAnalysis;

namespace Heyworks.PocketShooter.Realtime.Configuration
{
    /// <inheritdoc cref="IRealtimeConfiguration" />
    public class RealtimeConfiguration : IRealtimeConfiguration
    {
        private bool enableInterpolator = true;

        private bool interpolateLostTicks = true;

        /// <inheritdoc cref="IRealtimeConfiguration"/>
        public int TickIntervalMs => Constants.TickIntervalMs;

        /// <inheritdoc />
        public bool DiffState => true;

        /// <inheritdoc />
        public bool ReliableCommands => false;

        /// <inheritdoc cref="IRealtimeConfiguration"/>
        public bool EnableInterpolator
        {
            get => enableInterpolator;
            private set => enableInterpolator = value;
        }

        /// <inheritdoc cref="IRealtimeConfiguration"/>
        public bool InterpolateLostTicks
        {
            get => interpolateLostTicks;
            private set => interpolateLostTicks = value;
        }

        /// <inheritdoc cref="IRealtimeConfiguration"/>
        public int MaximumTickerSkewInMilliseconds => MaximumTickerSkewInTicks * TickIntervalMs;

        /// <inheritdoc cref="IRealtimeConfiguration"/>
        public int StatesBufferSize => Constants.BufferSize;

        /// <summary>
        /// Gets the maximum allowed ticker mal-synchronization in ticks.
        /// Should be less than <see cref="StatesBufferSize"/> / 2, because client and world tickers skew in opposite directions,
        /// so the gap can be twice as big.
        /// </summary>
        public int MaximumTickerSkewInTicks => StatesBufferSize / 2 - 1;

#if !UNITY_RELEASE
        [SuppressMessage("StyleCop", "SA1600", Justification = "Not release")]
        public void ToggleInterpolator()
        {
            EnableInterpolator = !EnableInterpolator;
        }

        [SuppressMessage("StyleCop", "SA1600", Justification = "Not release")]
        public void ToggleInterpolateLostTicks()
        {
            InterpolateLostTicks = !InterpolateLostTicks;
        }
#endif
    }
}