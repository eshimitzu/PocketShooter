using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Represents owned knife component.
    /// </summary>
    public sealed class NoAmmoOwnedWeapon : OwnedWeapon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoAmmoOwnedWeapon" /> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        /// <param name="weaponInfo">The information about weapon.</param>
        public NoAmmoOwnedWeapon(IRef<PlayerState> playerStateRef, WeaponInfo weaponInfo)
            : base(playerStateRef, weaponInfo)
        {
        }
    }
}