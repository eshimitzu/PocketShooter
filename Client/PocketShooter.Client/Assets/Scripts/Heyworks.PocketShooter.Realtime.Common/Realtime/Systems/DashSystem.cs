using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class DashSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public DashSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.Effects.Dash.IsDashing && ticker.Current >= player.DashExpire.ExpireAt)
            {
                player.Effects.Dash.IsDashing = false;

                return true;
            }

            return false;
        }
    }
}