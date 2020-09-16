using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems
{
    /// <summary>
    /// Represents interface for weapon that is used by warming up systems.
    /// </summary>
    internal interface IWarmingUpWeaponForSystem : IWeaponForSystem
    {
        /// <summary>
        /// Gets or sets warmup state.
        /// </summary>
        WarmupWeaponState WarmupWeaponState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets warmup progress.
        /// </summary>
        float WarmupProgress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether progress should be reset on shot.
        /// </summary>
        bool ResetProgressOnShot
        {
            get;
        }

        /// <summary>
        /// Gets warming speed.
        /// </summary>
        float WarmingSpeed { get; }

        /// <summary>
        /// Gets cooling speed.
        /// </summary>
        float CoolingSpeed { get; }
    }
}
