namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Root state component.
    /// </summary>
    public struct RootComponents
    {
        public RootBaseComponent Base;

        public RootExpireComponent Expire;
    }

    public struct RootBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is rooted.
        /// </summary>
        public bool IsRooted;
    }

    public struct RootExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when root should expired.
        /// </summary>
        public int ExpireAt;
    }
}
