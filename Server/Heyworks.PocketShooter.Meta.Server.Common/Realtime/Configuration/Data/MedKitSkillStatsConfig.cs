namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class MedKitSkillStatsConfig : SkillStatsConfig
    {
        /// <summary>
        /// Gets the number of first aid kits can be used per one spawn.
        /// </summary>
        public int AvailablePerSpawn { get; set; }

        /// <summary>
        /// Gets a health restored per <see cref="HealIntervalMs"/>.
        /// </summary>
        public float HealingPercent { get; set; }

        /// <summary>
        /// How often health restored.
        /// </summary>
        public int HealIntervalMs { get; set; }

        /// <summary>
        /// Gets the amount of heal intervals per kit usage.
        /// </summary>
        public int HealIntervalsPerUsage { get; set; }
    }
}