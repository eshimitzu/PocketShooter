namespace Heyworks.PocketShooter.Realtime.Channels
{
    public sealed class LeaveGameMessage : IServiceMessage
    {
        public LeaveGameMessage(PlayerId playerId)
        {
            PlayerId = playerId;
        }

        public PlayerId PlayerId { get; }
    }
}
