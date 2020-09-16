namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class ReloadCommandData : IGameCommandData
    {
        public ReloadCommandData(EntityId playerId)
        {
            ActorId = playerId;
        }

        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.Reload;
    }
}
