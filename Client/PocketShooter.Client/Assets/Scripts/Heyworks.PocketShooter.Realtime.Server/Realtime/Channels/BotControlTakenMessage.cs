using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    // is send to real player (device) so it can start sending command on bots behalf
    public sealed class BotControlTakenMessage : IMessage
    {
        public BotControlTakenMessage(PlayerInfo botInfo, TeamNo teamNo, EntityId trooperId)
        {
            BotInfo = botInfo;
            TeamNo = teamNo;
            TrooperId = trooperId;
        }
        public PlayerInfo BotInfo { get; set; }

        public TeamNo TeamNo { get; }

        public EntityId TrooperId { get; }
    }
}