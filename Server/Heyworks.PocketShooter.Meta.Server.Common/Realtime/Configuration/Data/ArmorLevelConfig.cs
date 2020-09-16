namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class ArmorLevelConfig
    {
        /// <summary>
        /// Gets or sets the armor name.
        /// </summary>
        public ArmorName Name { get; set; }

        /// <summary>
        /// Gets or sets the armor level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets an armor stats.
        /// </summary>
        public ArmorStatsConfig Stats { get; set; }
    }
}