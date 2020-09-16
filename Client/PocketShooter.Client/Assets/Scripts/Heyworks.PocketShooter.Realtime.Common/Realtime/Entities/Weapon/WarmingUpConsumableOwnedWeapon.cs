using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Owned weapon with warmup.
    /// </summary>
    public sealed class WarmingUpConsumableOwnedWeapon : OwnedWeapon, IWarmingUpWeapon, IConsumableWeapon, IConsumableWeaponForSystem, IWarmingUpWeaponForSystem
    {
        private readonly WarmingUpWeaponInfo warmingUpWeaponInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarmingUpConsumableOwnedWeapon" /> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        /// <param name="weaponInfo">The information about weapon.</param>
        public WarmingUpConsumableOwnedWeapon(IRef<PlayerState> playerStateRef, WarmingUpWeaponInfo weaponInfo)
            : base(playerStateRef, weaponInfo)
        {
            warmingUpWeaponInfo = weaponInfo;
        }

        /// <inheritdoc />
        public WarmupWeaponState WarmupWeaponState => Warmup.State;

        /// <inheritdoc />
        public float WarmupProgress => Warmup.Progress;

        /// <inheritdoc />
        public int AmmoInClip => Consumable.AmmoInClip;

        /// <inheritdoc />
        int IConsumableWeaponForSystem.AmmoInClip
        {
            get => Consumable.AmmoInClip;
            set => Consumable.AmmoInClip = value;
        }

        /// <inheritdoc />
        float IWarmingUpWeaponForSystem.WarmupProgress
        {
            get => Warmup.Progress;
            set => Warmup.Progress = value;
        }

        /// <inheritdoc />
        WarmupWeaponState IWarmingUpWeaponForSystem.WarmupWeaponState
        {
            get => Warmup.State;
            set => Warmup.State = value;
        }

        /// <inheritdoc />
        bool IWarmingUpWeaponForSystem.ResetProgressOnShot => warmingUpWeaponInfo.ResetProgressOnShot;

        /// <inheritdoc />
        float IWarmingUpWeaponForSystem.WarmingSpeed => warmingUpWeaponInfo.WarmingSpeed;

        /// <inheritdoc />
        float IWarmingUpWeaponForSystem.CoolingSpeed => warmingUpWeaponInfo.CoolingSpeed;

        private ref WeaponWarmupComponent Warmup => ref playerStateRef.Value.Weapon.Warmup;

        private ref WeaponConsumableComponent Consumable => ref playerStateRef.Value.Weapon.Consumable;
    }
}
