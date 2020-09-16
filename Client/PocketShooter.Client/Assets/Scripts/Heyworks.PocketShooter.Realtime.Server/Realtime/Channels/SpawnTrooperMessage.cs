namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal sealed class SpawnTrooperMessage : IServiceMessage
    {
        public SpawnTrooperMessage(PlayerId playerId, TrooperClass trooperClass)
        {
            PlayerId = playerId;
            TrooperClass = trooperClass;
        }

        public PlayerId PlayerId { get; }

        public TrooperClass TrooperClass { get; }
    }
}