namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// State of armor.
    /// </summary>
    public struct ArmorComponent : IForAll
    {
        /// <summary>
        /// Gets or sets a max armor value.
        /// </summary>
        public float MaxArmor;

        /// <summary>
        /// Gets or sets the armor.
        /// </summary>
        /// <value>
        /// The armor.
        /// </value>
        public float Armor;
    }
}