namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Health state.
    /// </summary>
    public struct HealthComponent : IForAll
    {
        /// <summary>
        /// Gets or sets a max health value.
        /// </summary>
        public float MaxHealth;

        /// <summary>
        /// Gets or sets current the health value.
        /// </summary>
        public float Health;
    }
}