using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.DataHandlers
{
    public class JoinRoomCommandDataHandler : IDataHandler
    {
        private static readonly IDataSerializer<JoinRoomCommandData> serializer = new JoinRoomCommandDataSerializer();

        private readonly IGameManagementChannel gameManagementChannel;
        private readonly ChannelWriter<IMessage> playerChannel;

        public JoinRoomCommandDataHandler(IGameManagementChannel gameManagementChannel, ChannelWriter<IMessage> playerChannel)
        {
            this.gameManagementChannel = gameManagementChannel;
            this.playerChannel = playerChannel;
        }

        public bool CanHandleData(byte dataCode) =>
            dataCode == (byte)NetworkDataCode.JoinRoomCommand;

        public void HandleData(byte dataCode, byte[] data)
        {
            var joinData = serializer.Deserialize(data);
            gameManagementChannel.JoinRoom(joinData.PlayerId, playerChannel);
        }
    }
}
