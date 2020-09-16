using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Shared by all weapon types by all remotes, clients and servers. Readonly.
    /// </summary>
    public interface IWeapon : IEntity<EntityId>
    {
        /// <summary>
        /// Gets current weapon state.
        /// </summary>
        WeaponState State { get; }

        /// <summary>
        /// Gets current weapon name.
        /// </summary>
        WeaponName Name { get; }
    }
}