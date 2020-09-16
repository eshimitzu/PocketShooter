using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.Channels;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents the client state when it enters to the game.
    /// </summary>
    internal sealed class ClientInGameState : ClientState
    {
        private readonly PlayerId playerId;
        private readonly ChannelWriter<IMessage> roomChannel;

        public ClientInGameState(
            PlayerId playerId,
            ChannelWriter<IMessage> roomChannel,
            IDataDispatcher dataDispatcher,
            IMessageDispatcher messageDispatcher,
            ILogger logger)
            : base(dataDispatcher, messageDispatcher, logger)
        {
            this.playerId = playerId;
            this.roomChannel = roomChannel;
        }

        /// <summary>
        /// Gets the state name.
        /// </summary>
        public override string Name => StateNames.ClientInGame;

        public override void HandleDisconnect()
        {
            roomChannel.WriteAsync(new LeaveGameMessage(playerId));
            base.HandleDisconnect();
        }
    }
}
