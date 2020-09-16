namespace Heyworks.PocketShooter.Realtime.Configuration
{
    /// <summary>
    /// Configuration properties for realtime gameplay networking.
    /// </summary>
    public interface IRealtimeConfiguration
    {
        #region constants used by realtime server and client

        /// <summary>
        /// Gets simulation tick interval.
        /// </summary>
        int TickIntervalMs { get; }

        /// <summary>
        /// Gets the size of the simulation states buffers.
        /// </summary>
        int StatesBufferSize { get; }

        #endregion constants used by realtime server and client

        #region client only params, may be sent from server as override

        /// <summary>
        /// Gets a value indicating whether interpolator is enabled.
        /// </summary>
        bool EnableInterpolator { get; }

        /// <summary>
        /// Gets a value indicating whether lost ticks should be interpolated.
        /// </summary>
        bool InterpolateLostTicks { get; }

        /// <summary>
        /// Should send client commands over reliable, or send unreliable but with resending for some range of previous ticks.
        /// </summary>
        bool ReliableCommands { get; }

        /// <summary>
        /// True if should ask server to do diff compression.
        /// </summary>
        bool DiffState { get; }

        #endregion client only params, may be sent from server as override

        #region client constants

        /// <summary>
        /// Gets the maximum allowed ticker mal-synchronization in ticks.
        /// Should be less than <see cref="StatesBufferSize"/> / 2, because client and world tickers skew in opposite directions,
        /// so the gap can be twice as big.
        /// </summary>
        int MaximumTickerSkewInTicks { get; }

        /// <summary>
        /// Gets the maximum allowed ticker mal-synchronization in milliseconds.
        /// </summary>
        int MaximumTickerSkewInMilliseconds { get; }

        #endregion client constants
    }
}