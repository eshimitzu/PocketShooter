namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class GameJoinedData
    {
        public GameJoinedData(DominationModeInfo modeInfo, RoomId roomId, TeamNo teamNo, PlayerInfo playerInfo, EntityId trooperId, int tick, int timeStamp)
        {
            ModeInfo = modeInfo;
            RoomId = roomId;
            TeamNo = teamNo;
            PlayerInfo = playerInfo;
            TrooperId = trooperId;
            Tick = tick;
            TimeStamp = timeStamp;
        }

        public DominationModeInfo ModeInfo { get; }

        public RoomId RoomId { get; }

        public TeamNo TeamNo { get; }

        public PlayerInfo PlayerInfo { get; }

        public EntityId TrooperId { get; }

        public int Tick { get; }

        public int TimeStamp { get; }
    }
}