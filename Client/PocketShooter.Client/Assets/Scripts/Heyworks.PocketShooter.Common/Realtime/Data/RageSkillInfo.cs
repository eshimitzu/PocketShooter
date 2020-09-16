namespace Heyworks.PocketShooter.Realtime.Data
{
    public class RageSkillInfo : SkillInfo
    {
        public RageSkillInfo()
        {
        }

        public RageSkillInfo(
            SkillInfo copyFrom,
            int increaseDamagePercent,
            int decreaseDamagePercent,
            int increaseDamageInterval,
            int decreaseDamageInterval)
            : base(copyFrom)
        {
            IncreaseDamagePercent = increaseDamagePercent;
            DecreaseDamagePercent = decreaseDamagePercent;
            IncreaseDamageInterval = increaseDamageInterval;
            DecreaseDamageInterval = decreaseDamageInterval;
        }

        public int IncreaseDamagePercent { get; set; }

        public int DecreaseDamagePercent { get; set; }

        public int IncreaseDamageInterval { get; set; }

        public int DecreaseDamageInterval { get; set; }
    }
}