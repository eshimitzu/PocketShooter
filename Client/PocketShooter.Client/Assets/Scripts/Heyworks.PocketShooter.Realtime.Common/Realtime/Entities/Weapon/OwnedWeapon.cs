using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Represents owned component for the weapon.
    /// </summary>
    public abstract class OwnedWeapon : WeaponBase, IOwnedWeapon, IWeaponForSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnedWeapon" /> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        /// <param name="weaponInfo">The information about weapon.</param>
        protected OwnedWeapon(IRef<PlayerState> playerStateRef, WeaponInfo weaponInfo)
            : base(playerStateRef)
        {
            Info = weaponInfo;
        }

        /// <summary>
        /// Gets the information about weapon.
        /// </summary>
        public WeaponInfo Info { get; private set; }

        /// <summary>
        /// Gets or sets current weapon state.
        /// </summary>
        WeaponState IWeaponForSystem.State
        {
            get => State;
            set => State = value;
        }

        /// <summary>
        /// Gets or sets a timestamp when weapon state should expired.
        /// </summary>
        int IWeaponForSystem.StateExpireAt
        {
            get => StateExpireAt;
            set => StateExpireAt = value;
        }

        public void ApplyState(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
        }

        private int StateExpireAt
        {
            get => StateExpire.ExpireAt;
            set => StateExpire.ExpireAt = value;
        }

        private ref WeaponStateExpireComponent StateExpire => ref playerStateRef.Value.Weapon.StateExpire;
    }
}