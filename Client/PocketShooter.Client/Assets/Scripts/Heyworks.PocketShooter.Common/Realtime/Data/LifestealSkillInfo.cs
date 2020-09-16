namespace Heyworks.PocketShooter.Realtime.Data
{
    public class LifestealSkillInfo : SkillInfo
    {
        public LifestealSkillInfo()
        {
        }

        public LifestealSkillInfo(SkillInfo copyFrom, int stealPeriod, float stealPercent, AoEInfo aoeInfo)
            : base(copyFrom)
        {
            StealPeriod = stealPeriod;
            StealPercent = stealPercent;
            AoE = aoeInfo;
        }

        public int StealPeriod { get; set; }

        public float StealPercent { get; set; }

        public AoEInfo AoE { get; set; }
    }
}