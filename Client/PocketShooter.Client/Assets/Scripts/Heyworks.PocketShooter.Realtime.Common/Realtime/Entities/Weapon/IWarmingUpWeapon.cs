using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Represents interface for warmup consumable owned weapon.
    /// </summary>
    public interface IWarmingUpWeapon : IWeapon
    {
        /// <summary>
        /// Gets warmup state.
        /// </summary>
        WarmupWeaponState WarmupWeaponState
        {
            get;
        }

        /// <summary>
        /// Gets warmup progress.
        /// </summary>
        float WarmupProgress
        {
            get;
        }
    }
}
