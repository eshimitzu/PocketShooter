namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal class RequestBotControlMessage : IManagementMessage
    {
        public RequestBotControlMessage(PlayerId playerId)
        {
            PlayerId = playerId;
        }

        public PlayerId PlayerId { get; }
    }
}
