namespace Heyworks.PocketShooter.Realtime.Data
{
    public class SprintSkillInfo : SkillInfo
    {
        public SprintSkillInfo()
        {
        }

        public SprintSkillInfo(SkillInfo copyFrom, float speedMultiplier)
            : base(copyFrom)
        {
            SpeedMultiplier = speedMultiplier;
        }

        public float SpeedMultiplier { get; set; }
    }
}