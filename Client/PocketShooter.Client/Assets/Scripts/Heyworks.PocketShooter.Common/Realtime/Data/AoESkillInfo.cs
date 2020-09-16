namespace Heyworks.PocketShooter.Realtime.Data
{
    public class AoESkillInfo : SkillInfo
    {
        public AoESkillInfo()
        {
        }

        public AoESkillInfo(SkillInfo copyFrom, int castingTime, AoEInfo aoeInfo)
            : base(copyFrom)
        {
            CastingTime = castingTime;
            AoE = aoeInfo;
        }

        public int CastingTime { get; set; }

        public AoEInfo AoE { get; set; }
    }
}