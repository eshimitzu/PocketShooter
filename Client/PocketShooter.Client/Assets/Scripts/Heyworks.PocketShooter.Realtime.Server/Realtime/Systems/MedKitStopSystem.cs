using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class MedKitStopSystem : ServerInitiatorSystem
    {
        private readonly ITicker ticker;

        public MedKitStopSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            if (initiator.Effects.MedKit.IsHealing &&
                (initiator.IsDead || ticker.Current >= initiator.MedKit.ExpiredAt))
            {
                initiator.Effects.MedKit.IsHealing = false;
            }
        }
    }
}
