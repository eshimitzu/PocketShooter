using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class LuckyDiscardSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuckyDiscardSystem"/> class.
        /// </summary>
        public LuckyDiscardSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        /// <inheritdoc/>
        public override bool Execute(OwnedPlayer player)
        {
            if (player.Effects.Lucky.IsLucky && ticker.Current >= player.LuckyExpire.ExpireAt)
            {
                player.Effects.Lucky.IsLucky = false;

                return true;
            }

            return false;
        }
    }
}
