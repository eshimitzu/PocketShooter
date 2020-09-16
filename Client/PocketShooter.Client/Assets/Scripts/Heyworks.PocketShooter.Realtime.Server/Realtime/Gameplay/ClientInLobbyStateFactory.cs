using System;
using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.DataHandlers;
using Heyworks.PocketShooter.Realtime.MessageProcessors;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents a factory that responsible for creating a <see cref="ClientInLobbyState"/> states.
    /// </summary>
    public class ClientInLobbyStateFactory : IClientStateFactory
    {
        private readonly IPeer peer;
        private readonly ChannelWriter<IMessage> playerChannel;
        private readonly ILoggerFactory loggerFactory;
        private readonly IGameManagementChannel gameManagementChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientInLobbyStateFactory"/> class.
        /// </summary>
        public ClientInLobbyStateFactory(
            IPeer peer,
            IGameManagementChannel gameManagementChannel,
            ChannelWriter<IMessage> playerChannel,
            ILoggerFactory loggerFactory)
        {
            this.peer = peer;
            this.gameManagementChannel = gameManagementChannel;
            this.playerChannel = playerChannel;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates client state.
        /// </summary>
        /// <param name="message">The message.</param>
        public IClientState CreateState(IMessage message)
        {
            if (message is LobbyEnteredMessage)
            {
                var dataDispatcher = new DataDispatcher();
                dataDispatcher.AddDataHandler(new JoinRoomCommandDataHandler(gameManagementChannel, playerChannel));

                var messageDispatcher = new MessageDispatcher();
                messageDispatcher.AddMessageProcessor(new GameJoinedMessageProcessor(peer));
                messageDispatcher.AddMessageProcessor(new InLobbyErrorMessageProcessor(peer));
                var logger = loggerFactory.CreateLogger<ClientInLobbyState>();

                return new ClientInLobbyState(dataDispatcher, messageDispatcher, logger);
            }

            throw new NotImplementedException($"The message of type {message.GetType()} is not supported");
        }
    }
}
