namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Represents lucky components.
    /// </summary>
    public struct LuckyComponents
    {
        /// <summary>
        /// Base.
        /// </summary>
        public LuckyBaseComponent Base;

        /// <summary>
        /// Expire.
        /// </summary>
        public LuckyExpireComponent Expire;
    }

    /// <summary>
    /// LuckyComponentsBaseComponent.
    /// </summary>
    public struct LuckyBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is lucky.
        /// </summary>
        public bool IsLucky;
    }

    /// <summary>
    /// Represents lucky expire component.
    /// </summary>
    public struct LuckyExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when lucky should expired.
        /// </summary>
        public int ExpireAt;
    }
}
