namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Weapon only components.
    /// </summary>
    public struct WeaponComponents
    {
        /// <summary>
        /// The base weapon data.
        /// </summary>
        public WeaponBaseComponent Base;

        /// <summary>
        /// The consumable data.
        /// </summary>
        public WeaponConsumableComponent Consumable;

        /// <summary>
        /// Warmup data.
        /// </summary>
        public WeaponWarmupComponent Warmup;

        /// <summary>
        /// The tick to cancel state at.
        /// </summary>
        public WeaponStateExpireComponent StateExpire;
    }

    public struct WeaponStateExpireComponent : IForOwner
    {
        /// <summary>
        /// Timestamp when weapon state should expired.
        /// </summary>
        public int ExpireAt;
    }

    public struct WeaponConsumableComponent : IForOwner
    {
        /// <summary>
        /// Gets or sets the ammo in clip.
        /// </summary>
        public int AmmoInClip;
    }

    /// <summary>
    /// Weapon base part.
    /// </summary>
    public struct WeaponBaseComponent : IForAll
    {
        /// <summary>
        /// Current weapon state.
        /// </summary>
        public WeaponState State;

        /// <summary>
        /// Current weapon name.
        /// </summary>
        public WeaponName Name;

        public override string ToString() => (Name, State).ToString();
    }

    /// <summary>
    /// Warmup components for all.
    /// </summary>
    public struct WeaponWarmupComponent : IForAll
    {
        /// <summary>
        /// Warmup progress.
        /// </summary>
        public float Progress;

        /// <summary>
        /// Warmup state.
        /// </summary>
        public WarmupWeaponState State;
    }
}