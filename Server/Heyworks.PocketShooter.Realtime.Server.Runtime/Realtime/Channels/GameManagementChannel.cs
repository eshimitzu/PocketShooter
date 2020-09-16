using System.Collections.Generic;
using System.Threading.Channels;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal sealed class GameManagementChannel : IGameManagementChannel
    {
        private readonly Channel<IManagementMessage> channel;

        public GameManagementChannel()
        {
            this.channel = Channel.CreateUnbounded<IManagementMessage>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false,
                });
        }

        public ChannelReader<IManagementMessage> Reader => channel;

        // ISSUE: flawed - eats error (also same was with fibers)
        public void SendMessage(IManagementMessage message) => channel.Writer.WriteAsync(message);

        public void Complete() => channel.Writer.Complete();

        void IGameManagementChannel.JoinRoom(PlayerId playerId, ChannelWriter<IMessage> playerChannel) =>
            SendMessage(new JoinRoomMessage(playerId, playerChannel));

        void IGameManagementChannel.LeaveRoom(PlayerId playerId) =>
            SendMessage(new LeaveServerMessage(playerId));

        void IGameManagementChannel.CloseRoom(RoomId roomId) =>
            SendMessage(new CloseRoomMessage(roomId));

        void IGameManagementChannel.RequestBotControl(PlayerId playerId) =>
            SendMessage(new RequestBotControlMessage(playerId));

        void IGameManagementChannel.ApplyMatchResults(RoomId roomId, IReadOnlyList<PlayerMatchResults> matchResults) =>
            SendMessage(new ApplyMatchResultsMessage(roomId, matchResults));

        void IGameManagementChannel.UpdateConsumables(PlayerId playerId, int usedOffensives, int usedSupports) =>
            SendMessage(new UpdateConsumablesMessage(playerId, usedOffensives, usedSupports));
    }
}
