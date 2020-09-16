namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class BotControlTakenData
    {
        public BotControlTakenData(PlayerInfo botInfo, TeamNo teamNo, EntityId trooperId)
        {
            TeamNo = teamNo;
            BotInfo = botInfo;
            TrooperId = trooperId;
        }

        public PlayerInfo BotInfo { get; }

        public TeamNo TeamNo { get; }

        public EntityId TrooperId { get; }
    }
}