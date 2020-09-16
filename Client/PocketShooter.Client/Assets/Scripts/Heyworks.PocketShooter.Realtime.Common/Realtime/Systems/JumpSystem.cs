using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class JumpSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public JumpSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.Effects.Jump.IsJumping && ticker.Current >= player.JumpExpire.ExpireAt)
            {
                player.Effects.Jump.IsJumping = false;

                return true;
            }

            return false;
        }
    }
}
