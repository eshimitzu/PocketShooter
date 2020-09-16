using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class InvisibleSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public InvisibleSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (
                player.Effects.Invisible.IsInvisible &&
                (ticker.Current >= player.InvisibleExpire.ExpireAt))
            {
                player.Effects.Invisible.IsInvisible = false;

                return true;
            }

            return false;
        }
    }
}
