using System;
using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.DataHandlers;
using Heyworks.PocketShooter.Realtime.MessageProcessors;
using Heyworks.PocketShooter.Realtime.Channels;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents a factory that responsible for creating a <see cref="ClientInGameState"/> states.
    /// </summary>
    public class ClientInGameStateFactory : IClientStateFactory
    {
        private readonly IPeer peer;
        private readonly IGameManagementChannel gameManagementChannel;
        private readonly ChannelWriter<IMessage> playerChannel;
        private readonly ILoggerFactory loggerFactory;

        public ClientInGameStateFactory(
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
            if (message is GameJoinedMessage gjm)
            {
                var dataDispatcher = new DataDispatcher();
                dataDispatcher.AddDataHandler(new SimulationCommandDataHandler(
                    gjm.PlayerId,
                    gameManagementChannel,
                    gjm.RoomChannel,
                    playerChannel));

                var logger = loggerFactory.CreateLogger($"{typeof(ClientInGameState).FullName}<{gjm.RoomId}>");

                var messageDispatcher = new MessageDispatcher();

                messageDispatcher.AddMessageProcessor(new SimulationServiceMessageProcessor(peer, logger));
                messageDispatcher.AddMessageProcessor(new SimulationStateMessageProcessor(peer, logger));

                return new ClientInGameState(
                    gjm.PlayerId,
                    gjm.RoomChannel,
                    dataDispatcher,
                    messageDispatcher,
                    logger);
            }

            throw new NotImplementedException($"The message of type {message.GetType()} is not supported");
        }
    }
}
