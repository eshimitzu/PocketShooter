using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal class AddBotMessage : IHasRoomIdMessage
    {
        public AddBotMessage(RoomId roomId, PlayerInfo botInfo)
        {
            RoomId = roomId;
            BotInfo = botInfo;
        }

        public RoomId RoomId { get; }

        public PlayerInfo BotInfo { get; }
    }
}