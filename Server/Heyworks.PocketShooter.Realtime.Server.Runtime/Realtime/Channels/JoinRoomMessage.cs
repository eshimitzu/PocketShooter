using System.Threading.Channels;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal sealed class JoinRoomMessage : IManagementMessage
    {
        public JoinRoomMessage(PlayerId playerId, ChannelWriter<IMessage> playerChannel)
        {
            PlayerId = playerId;
            PlayerChannel = playerChannel;
        }

        public PlayerId PlayerId { get; }

        public ChannelWriter<IMessage> PlayerChannel { get; }
    }
}
