namespace Heyworks.PocketShooter.Realtime.Service
{
    public interface IRttProvider
    {
        /// <summary>
        /// Gets client-server round-trip time.
        /// </summary>
        int RoundTripTimeMs { get; }

        /// <summary>
        /// Gets last client-server round-trip time.
        /// </summary>
        int LastRoundTripTimeMs { get; }

        /// <summary>
        /// Gets the round trip time variance.
        /// </summary>
        int RoundTripTimeVariance { get; }

        /// <summary>
        /// Gets the server time in milliseconds.
        /// </summary>
        int ServerTimeMs { get; }
    }
}