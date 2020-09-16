using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Represents base class for all weapons.
    /// </summary>
    public abstract class WeaponBase : IWeapon
    {
        /// <summary>
        /// As of now weapon is one to one with player.
        /// </summary>
        public EntityId Id => playerStateRef.Value.Id;

        /// <summary>
        /// Gets or sets the player state.
        /// </summary>
        // TODO: a.dezhurko [weapon]: Replace with WeaponComponents
        protected IRef<PlayerState> playerStateRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponBase"/> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        /// <param name="config">The config.</param>
        protected WeaponBase(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
        }

        /// <summary>
        /// Gets current weapon state.
        /// </summary>
        public WeaponState State
        {
            get => Base.State;
            internal set => Base.State = value;
        }

        /// <summary>
        /// Gets current weapon name.
        /// </summary>
        public WeaponName Name
        {
            get => Base.Name;
            internal set => Base.Name = value;
        }

        private ref WeaponBaseComponent Base => ref playerStateRef.Value.Weapon.Base;
    }
}
