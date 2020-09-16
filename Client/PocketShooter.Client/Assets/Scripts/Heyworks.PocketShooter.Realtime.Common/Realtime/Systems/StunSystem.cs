using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class StunSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        /// <summary>
        /// Initializes a new instance of the <see cref="StunSystem"/> class.
        /// </summary>
        public StunSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        /// <inheritdoc/>
        public override bool Execute(OwnedPlayer player)
        {
            if (player.Effects.Stun.IsStunned && ticker.Current >= player.StunExpire.ExpireAt)
            {
                player.Effects.Stun.IsStunned = false;

                return true;
            }

            return false;
        }
    }
}
