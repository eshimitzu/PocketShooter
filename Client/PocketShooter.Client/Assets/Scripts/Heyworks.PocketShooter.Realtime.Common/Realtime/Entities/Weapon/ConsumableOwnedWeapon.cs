using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Represents weapon that has ammo.
    /// </summary>
    public sealed class ConsumableOwnedWeapon : OwnedWeapon, IConsumableWeapon, IConsumableWeaponForSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumableOwnedWeapon" /> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        /// <param name="weaponInfo">The information about weapon.</param>
        public ConsumableOwnedWeapon(IRef<PlayerState> playerStateRef, WeaponInfo weaponInfo)
            : base(playerStateRef, weaponInfo)
        {
        }

        /// <summary>
        /// Gets the ammo in clip.
        /// </summary>
        public int AmmoInClip => Consumable.AmmoInClip;

        /// <summary>
        /// Gets or sets the ammo in clip.
        /// </summary>
        int IConsumableWeaponForSystem.AmmoInClip
        {
            get => Consumable.AmmoInClip;
            set => Consumable.AmmoInClip = value;
        }

        /// <summary>
        /// Resets the weapon.
        /// </summary>
        public void Reset()
        {
            Consumable.AmmoInClip = Info.ClipSize;
            State = WeaponState.Default;
        }

        private ref WeaponConsumableComponent Consumable => ref playerStateRef.Value.Weapon.Consumable;
    }
}