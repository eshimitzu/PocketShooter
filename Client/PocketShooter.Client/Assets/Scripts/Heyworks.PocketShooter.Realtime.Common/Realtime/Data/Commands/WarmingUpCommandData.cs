namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class WarmingUpCommandData : IGameCommandData
    {
        public WarmingUpCommandData(EntityId playerId)
        {
            ActorId = playerId;
        }

        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.WarmingUp;
    }
}
