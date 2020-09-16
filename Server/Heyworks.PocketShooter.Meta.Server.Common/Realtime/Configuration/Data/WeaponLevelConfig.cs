namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class WeaponLevelConfig
    {
        /// <summary>
        /// Gets or sets the weapon name.
        /// </summary>
        public WeaponName Name { get; set; }

        /// <summary>
        /// Gets or sets the weapon level.
        /// </summary>
        public int Level { get; set; }

        public WeaponStatsConfig Stats { get; set; }
    }
}