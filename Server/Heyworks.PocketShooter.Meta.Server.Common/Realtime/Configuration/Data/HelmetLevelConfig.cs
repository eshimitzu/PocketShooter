namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class HelmetLevelConfig
    {
        /// <summary>
        /// Gets or sets the helmet name.
        /// </summary>
        public HelmetName Name { get; set; }

        /// <summary>
        /// Gets or sets the helmet level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets a helmet stats.
        /// </summary>
        public HelmetStatsConfig Stats { get; set; }
    }
}