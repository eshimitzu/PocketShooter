namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents interface for network statistics.
    /// </summary>
    public interface ISimulationStatsProvider
    {
        /// <summary>
        /// Gets the current simulating tick.
        /// </summary>
        int Tick { get; }

        /// <summary>
        /// Gets the world tick.
        /// </summary>
        int WorldTick { get; }

        /// <summary>
        /// Gets the round trip time.
        /// </summary>
        int RoundTripTime { get; }

        /// <summary>
        /// Gets the round trip time variance.
        /// </summary>
        int RoundTripTimeVariance { get; }

        /// <summary>
        /// Gets the last received world tick.
        /// </summary>
        int LastReceivedWorldTick { get; }

        /// <summary>
        /// Gets the mean size of the client commands buffer on server.
        /// </summary>
        double InputBufferSizeOnServerMean { get; }

        /// <summary>
        /// Gets the size variance of the client commands buffer on server.
        /// </summary>
        double InputBufferSizeOnServerVariance { get; }

        /// <summary>
        /// Gets the last size value of the client commands buffer on server.
        /// </summary>
        int InputBufferSizeOnServer { get; }

        /// <summary>
        /// Gets the target size of the client commands buffer on server.
        /// </summary>
        double TargetInputBufferSizeOnServer { get; }

        /// <summary>
        /// Gets the mean size of the world states buffer on client.
        /// </summary>
        double WorldStatesBufferSizeMean { get; }

        /// <summary>
        /// Gets the size variance of the world states buffer on client.
        /// </summary>
        double WorldStatesBufferSizeVariance { get; }

        /// <summary>
        /// Gets the current size value of the world states buffer on client.
        /// </summary>
        int WorldStatesBufferSize { get; }

        /// <summary>
        /// Gets the target size of the world states buffer on client.
        /// </summary>
        double TargetWorldStatesBufferSize { get; }
    }
}