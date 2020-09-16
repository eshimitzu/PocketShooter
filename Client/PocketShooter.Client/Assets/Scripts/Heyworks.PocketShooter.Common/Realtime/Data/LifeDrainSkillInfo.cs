namespace Heyworks.PocketShooter.Realtime.Data
{
    public class LifeDrainSkillInfo : SkillInfo
    {
        public LifeDrainSkillInfo()
        {
        }

        public LifeDrainSkillInfo(SkillInfo copyFrom, float drainPercent)
            : base(copyFrom)
        {
            DrainPercent = drainPercent;
        }

        public float DrainPercent { get; set; }
    }
}