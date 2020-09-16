using Heyworks.PocketShooter.Realtime.Data;
using System.Threading.Channels;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    public sealed class GameJoinedMessage : IMessage
    {
        public GameJoinedMessage(
            DominationModeInfo modeInfo,
            TeamNo teamNo,
            PlayerInfo playerInfo,
            EntityId trooperId,
            int simulationTick,
            ChannelWriter<IMessage> roomChannel,
            RoomId roomId)
        {
            ModeInfo = modeInfo;
            TeamNo = teamNo;
            PlayerInfo = playerInfo;
            TrooperId = trooperId;
            SimulationTick = simulationTick;
            RoomChannel = roomChannel;
            RoomId = roomId;
        }

        public DominationModeInfo ModeInfo { get;  }

        public TeamNo TeamNo { get; }

        public PlayerInfo PlayerInfo { get; }

        public PlayerId PlayerId => PlayerInfo.Id;

        public EntityId TrooperId { get; }

        public int SimulationTick { get; }

        public ChannelWriter<IMessage> RoomChannel { get; }

        public RoomId RoomId { get; }
    }
}