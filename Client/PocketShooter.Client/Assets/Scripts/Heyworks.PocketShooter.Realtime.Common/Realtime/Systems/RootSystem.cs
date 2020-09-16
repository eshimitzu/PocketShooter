using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class RootSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public RootSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.Effects.Root.IsRooted && ticker.Current >= player.RootExpire.ExpireAt)
            {
                player.Effects.Root.IsRooted = false;

                return true;
            }

            return false;
        }
    }
}
