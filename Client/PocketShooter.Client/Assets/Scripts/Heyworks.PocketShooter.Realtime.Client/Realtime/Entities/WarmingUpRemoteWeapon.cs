using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class WarmingUpRemoteWeapon : RemoteWeapon, IWarmingUpWeapon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarmingUpRemoteWeapon"/> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        public WarmingUpRemoteWeapon(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        /// <summary>
        /// Gets warmup state.
        /// </summary>
        public WarmupWeaponState WarmupWeaponState => Warmup.State;

        /// <summary>
        /// Gets warmup progress.
        /// </summary>
        public float WarmupProgress => Warmup.Progress;

        private ref WeaponWarmupComponent Warmup => ref playerStateRef.Value.Weapon.Warmup;
    }
}
