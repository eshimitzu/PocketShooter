using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems
{
    /// <summary>
    /// Represents interface for weapon entity that is used by weapon systems.
    /// </summary>
    public interface IWeaponForSystem
    {
        /// <summary>
        /// Gets or sets current weapon state.
        /// </summary>
        WeaponState State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a timestamp when current state expired.
        /// </summary>
        int StateExpireAt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the information about weapon.
        /// </summary>
        WeaponInfo Info
        {
            get;
        }
    }
}
