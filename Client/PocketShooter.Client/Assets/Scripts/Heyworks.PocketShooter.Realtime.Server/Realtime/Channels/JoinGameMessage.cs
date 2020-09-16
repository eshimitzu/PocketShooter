using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    public class JoinGameMessage : IServiceMessage
    {
        public JoinGameMessage(PlayerInfo playerInfo, ChannelWriter<IMessage> playerChannel)
        {
            PlayerInfo = playerInfo;
            PlayerChannel = playerChannel;
        }

        public PlayerInfo PlayerInfo { get; }

        public ChannelWriter<IMessage> PlayerChannel { get; }
    }
}
