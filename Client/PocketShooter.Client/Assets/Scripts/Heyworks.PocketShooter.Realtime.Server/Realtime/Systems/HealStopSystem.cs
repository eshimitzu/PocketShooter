using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class HealStopSystem : ServerInitiatorSystem
    {
        private readonly ITicker ticker;

        public HealStopSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            if (initiator.Effects.Heal.IsHealing &&
                (initiator.IsDead || ticker.Current >= initiator.Heal.ExpiredAt))
            {
                initiator.Effects.Heal.IsHealing = false;
            }
        }
    }
}
