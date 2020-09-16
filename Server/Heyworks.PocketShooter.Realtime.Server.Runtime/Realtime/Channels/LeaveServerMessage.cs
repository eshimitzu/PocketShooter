namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal class LeaveServerMessage : IManagementMessage
    {
        public LeaveServerMessage(PlayerId playerId)
        {
            PlayerId = playerId;
        }

        public PlayerId PlayerId { get; }
    }
}
