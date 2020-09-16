using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents the client state when it unauthorized.
    /// </summary>
    public sealed class ClientUnauthorizedState : IClientState
    {
        public ClientUnauthorizedState()
        {
        }

        /// <summary>
        /// Gets the state name.
        /// </summary>
        public string Name => StateNames.ClientUnauthorized;

        public void HandleData(byte dataCode, byte[] data) =>
            throw new System.NotImplementedException();

        public void HandleDisconnect() =>
            throw new System.NotImplementedException();

        public void ProcessMessage(IMessage message)
        {
        }
    }
}
