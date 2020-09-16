using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class ImmortalityStopSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public ImmortalityStopSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.Effects.Immortal.IsImmortal && ticker.Current >= player.ImmortalExpire.ExpireAt)
            {
                player.Effects.Immortal.IsImmortal = false;

                return true;
            }

            return false;
        }
    }
}