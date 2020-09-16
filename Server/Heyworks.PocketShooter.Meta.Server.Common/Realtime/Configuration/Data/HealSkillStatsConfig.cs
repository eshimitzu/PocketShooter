namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class HealSkillStatsConfig : SkillStatsConfig
    {
        /// <summary>
        /// Gets a health restored per <see cref="HealIntervalMs"/>.
        /// </summary>
        public float HealingPercent { get; set; }

        /// <summary>
        /// How often health restored.
        /// </summary>
        public int HealIntervalMs { get; set; }

        /// <summary>
        /// Gets the amount of heal intervals per usage.
        /// </summary>
        public int HealIntervalsPerUsage { get; set; }
    }
}