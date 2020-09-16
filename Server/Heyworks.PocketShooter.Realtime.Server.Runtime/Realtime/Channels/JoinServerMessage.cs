using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal class JoinServerMessage : IHasRoomIdMessage
    {
        public JoinServerMessage(RoomId roomId, PlayerInfo playerInfo)
        {
            RoomId = roomId;
            PlayerInfo = playerInfo;
        }

        public RoomId RoomId { get; }

        public PlayerInfo PlayerInfo { get; }
    }
}
