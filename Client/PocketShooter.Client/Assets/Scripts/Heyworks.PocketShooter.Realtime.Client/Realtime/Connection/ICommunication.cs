using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Service;

namespace Heyworks.PocketShooter.Realtime.Connection
{
    /// <summary>
    /// Represents communication interface.
    /// </summary>
    public interface ICommunication : IRttProvider
    {
        /// <summary>
        /// Queues the command to be send to the server on the <see cref="QueueData" /> call.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="reliable">True if should be send reliable.</param>
        void QueueData(NetworkData data, bool reliable);

        /// <summary>
        /// Sends queued commands to the server.
        /// </summary>
        void Send();

        /// <summary>
        /// Dispatches incoming messages.
        /// </summary>
        void Receive();

        /// <summary>
        /// Checks for available data.
        /// </summary>
        /// <returns>True if data is available, false otherwise.</returns>
        bool HasData();

        /// <summary>
        /// Gets recieved data.
        /// </summary>
        /// <returns>Network data.</returns>
        NetworkData GetData();
    }
}