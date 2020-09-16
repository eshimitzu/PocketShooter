using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents the client state when it enters to the game menu.
    /// </summary>
    public sealed class ClientInLobbyState : ClientState
    {
        public ClientInLobbyState(IDataDispatcher dataDispatcher, IMessageDispatcher messageDispatcher, ILogger logger)
            : base(dataDispatcher, messageDispatcher, logger)
        {
        }

        /// <summary>
        /// Gets the state name.
        /// </summary>
        public override string Name => StateNames.ClientInLobby;
    }
}
