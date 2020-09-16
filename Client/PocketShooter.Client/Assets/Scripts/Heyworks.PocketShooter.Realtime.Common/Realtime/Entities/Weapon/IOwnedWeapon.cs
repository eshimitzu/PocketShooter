using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Owned weapon.
    /// </summary>
    public interface IOwnedWeapon : IWeapon
    {
        /// <summary>
        /// Gets the information about weapon.
        /// </summary>
        WeaponInfo Info { get; }
    }
}