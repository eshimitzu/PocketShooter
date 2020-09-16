namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Invisible components.
    /// </summary>
    public struct InvisibleComponents
    {
        public InvisibleBaseComponent Base;

        public InvisibleExpireComponent Expire;
    }

    public struct InvisibleBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is invisible.
        /// </summary>
        public bool IsInvisible;
    }

    public struct InvisibleExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when invisible should expired.
        /// </summary>
        public int ExpireAt;
    }
}