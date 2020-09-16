using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.MessageProcessors
{
    public class GameJoinedMessageProcessor : IMessageProcessor
    {
        private static readonly IDataSerializer<GameJoinedData> serializer = new GameJoinedDataSerializer();

        private readonly IPeer peer;

        public GameJoinedMessageProcessor(IPeer peer) => this.peer = peer;

        public bool CanProcessMessage(IMessage message) => message is GameJoinedMessage;

        public void ProcessMessage(IMessage message)
        {
            var gjm = message as GameJoinedMessage;

            var data = new GameJoinedData(
                gjm.ModeInfo,
                gjm.RoomId,
                gjm.TeamNo,
                gjm.PlayerInfo,
                gjm.TrooperId,
                gjm.SimulationTick,
                Environment.TickCount);

            peer.SendEvent(NetworkDataCode.GameJoined, serializer.Serialize(data), false);
        }
    }
}
