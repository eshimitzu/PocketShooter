namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents interface for class that can provide information about buffers.
    /// </summary>
    public interface IBuffersStatsProvider
    {
        /// <summary>
        /// Gets the last received world tick.
        /// </summary>
        int LastReceivedWorldTick { get; }

        /// <summary>
        /// Gets the current size value of the world states buffer on client.
        /// </summary>
        int WorldStatesBufferSize { get; }

        /// <summary>
        /// Gets the last size value of the client commands buffer on server.
        /// </summary>
        int ServerInputBufferSize { get; }
    }
}
