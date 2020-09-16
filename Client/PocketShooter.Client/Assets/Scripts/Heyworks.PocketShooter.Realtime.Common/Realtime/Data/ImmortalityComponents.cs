namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Immortality components.
    /// </summary>
    public struct ImmortalityComponents
    {
        /// <summary>
        /// Base.
        /// </summary>
        public ImmortalityBaseComponent Base;

        /// <summary>
        /// Expire.
        /// </summary>
        public ImmortalityExpireComponent Expire;

        public override string ToString() => $"{nameof(ImmortalityComponents)}{(Base.IsImmortal, Expire.ExpireAt)}";
    }

    /// <summary>
    /// ImmortalityBaseComponent.
    /// </summary>
    public struct ImmortalityBaseComponent : IForAll
    {
        /// <summary>
        /// Gets if player is immortality.
        /// </summary>
        public bool IsImmortal;
    }

    /// <summary>
    /// ImmortalityExpireComponent.
    /// </summary>
    public struct ImmortalityExpireComponent : IForOwner
    {
        /// <summary>
        /// Gets timestamp when immortality should expired.
        /// </summary>
        public int ExpireAt;
    }
}