namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Stun state component.
    /// </summary>
    public struct StunComponents
    {
        public StunBaseComponent Base;

        public StunExpireComponent Expire;
    }

    public struct StunBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is stunned.
        /// </summary>
        public bool IsStunned;
    }

    public struct StunExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when stun should expired.
        /// </summary>
        public int ExpireAt;
    }
}
