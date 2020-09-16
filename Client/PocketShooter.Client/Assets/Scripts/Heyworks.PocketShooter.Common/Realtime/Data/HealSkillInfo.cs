namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Information about heal skill.
    /// </summary>
    public class HealSkillInfo : SkillInfo
    {
        public HealSkillInfo()
        {
        }

        public HealSkillInfo(
            SkillInfo copyFrom,
            float healingPercent,
            int healInterval,
            int healIntervalsPerUsage)
            : base(copyFrom)
        {
            HealingPercent = healingPercent;
            HealInterval = healInterval;
            RegenerationInterval = healInterval * healIntervalsPerUsage;
        }

        /// <summary>
        /// Gets a health restored per <see cref="HealInterval"/>.
        /// </summary>
        public float HealingPercent { get; set; }

        /// <summary>
        /// Gets a how often health restored.
        /// </summary>
        public int HealInterval { get; set; }

        /// <summary>
        /// Gets total time of regeneration per kit usage.
        /// </summary>
        public int RegenerationInterval { get; }
    }
}