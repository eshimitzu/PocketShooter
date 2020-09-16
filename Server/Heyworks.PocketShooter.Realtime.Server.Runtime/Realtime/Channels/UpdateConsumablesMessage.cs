namespace Heyworks.PocketShooter.Realtime.Channels
{
    public class UpdateConsumablesMessage : IManagementMessage
    {
        public UpdateConsumablesMessage(PlayerId playerId, int usedOffensives, int usedSupports)
        {
            PlayerId = playerId;
            UsedOffensives = usedOffensives;
            UsedSupports = usedSupports;
        }

        public PlayerId PlayerId { get; }

        public int UsedOffensives { get; }

        public int UsedSupports { get; }
    }
}
