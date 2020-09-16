namespace Heyworks.PocketShooter.Realtime.Data
{
    public class MedKitSkillInfo : SkillInfo
    {
        public MedKitSkillInfo()
        {
        }

        public MedKitSkillInfo(
            SkillInfo copyFrom,
            float healingPercent,
            int healInterval,
            int healIntervalsPerUsage,
            int availablePerSpawn)
            : base(copyFrom)
        {
            HealingPercent = healingPercent;
            HealInterval = healInterval;
            AvailablePerSpawn = availablePerSpawn;
            RegenerationInterval = healInterval * healIntervalsPerUsage;
        }

        /// <summary>
        /// Gets a health restored per <see cref="HealInterval"/>.
        /// </summary>
        public float HealingPercent { get; set; }

        /// <summary>
        /// Gets or sets a how often health restored.
        /// </summary>
        public int HealInterval { get; set; }

        /// <summary>
        /// Gets the number of first aid kits can be used per one spawn.
        /// </summary>
        public int AvailablePerSpawn { get; set; }

        /// <summary>
        /// Gets total time of regeneration per kit usage.
        /// </summary>
        public int RegenerationInterval { get; }
    }
}
