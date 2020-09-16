namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Dash effect component.
    /// </summary>
    public struct DashComponents
    {
        public DashBaseComponent Base;

        public DashExpireComponent Expire;
    }

    public struct DashBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is Dashing.
        /// </summary>
        public bool IsDashing;
    }

    public struct DashExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when Dash should expired.
        /// </summary>
        public int ExpireAt;
    }
}