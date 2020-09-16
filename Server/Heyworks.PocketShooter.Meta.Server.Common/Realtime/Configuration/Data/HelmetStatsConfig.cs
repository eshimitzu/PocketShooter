namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class HelmetStatsConfig
    {
        /// <summary>
        /// Gets or sets the impact for max health.
        /// </summary>
        public int MaxHealth { get; set; }

        public static HelmetStatsConfig Sum(HelmetStatsConfig first, HelmetStatsConfig second)
        {
            return new HelmetStatsConfig
            {
                MaxHealth= first.MaxHealth + second.MaxHealth,
            };
        }
    }
}
