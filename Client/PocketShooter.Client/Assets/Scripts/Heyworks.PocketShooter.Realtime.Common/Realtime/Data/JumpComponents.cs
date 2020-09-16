namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Jump components.
    /// </summary>
    public struct JumpComponents
    {
        public JumpBaseComponent Base;

        public JumpExpireComponent Expire;
    }

    public struct JumpBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is Jumping.
        /// </summary>
        public bool IsJumping;
    }

    public struct JumpExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when Jump should expired.
        /// </summary>
        public int ExpireAt;
    }
}