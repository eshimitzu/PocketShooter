using System.Linq;
using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Defines an interface for the client state.
    /// </summary>
    public interface IClientState
    {
        /// <summary>
        /// Gets the state name.
        /// </summary>
        string Name { get; }

        void HandleData(byte dataCode, byte[] data);

        void ProcessMessage(IMessage message);

        /// <summary>
        /// Method responsible for handling client disconnects.
        /// </summary>
        void HandleDisconnect();
    }
}
