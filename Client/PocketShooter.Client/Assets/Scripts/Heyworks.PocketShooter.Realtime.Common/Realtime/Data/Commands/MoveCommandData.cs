namespace Heyworks.PocketShooter.Realtime.Data
{
    public readonly struct MoveCommandData : IGameCommandData
    {
        public MoveCommandData(EntityId playerId, FpsTransformComponent transform)
        {
            this.Transform = transform;
            ActorId = playerId;
        }

        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.Move;

        public readonly FpsTransformComponent Transform;

        public override string ToString() => $"{nameof(MoveCommandData)}{(ActorId, Code, Transform)}";
    }
}
