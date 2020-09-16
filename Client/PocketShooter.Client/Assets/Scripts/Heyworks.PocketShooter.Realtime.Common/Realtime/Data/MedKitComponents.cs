namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct MedKitComponents
    {
        public MedKitBaseComponent Base;

        public MedKitOwnedComponent Owned;
    }

    public struct MedKitBaseComponent : IForAll
    {
        /// <summary>
        /// Time until current first aid kit is active.
        /// </summary>
        public bool IsHealing;
    }

    /// <summary>
    /// The state for first aid kit.
    /// </summary>
    public struct MedKitOwnedComponent : IForOwner
    {
        /// <summary>
        /// The timestamp when the next heal should occur.
        /// </summary>
        // TODO: v.shimkovich this is actually not _read_ by client logic or ui
        // and can be not synced and stored only on server.
        // But need to decouple MedKitStartSystem
        // ~IForServer
        public int NextHealAt;

        /// <summary>
        /// Time until current first aid kit is active.
        /// </summary>
        public int ExpiredAt;
    }
}