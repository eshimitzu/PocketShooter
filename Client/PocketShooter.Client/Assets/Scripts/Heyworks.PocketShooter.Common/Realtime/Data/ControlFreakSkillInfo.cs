namespace Heyworks.PocketShooter.Realtime.Data
{
    public class ControlFreakSkillInfo : SkillInfo
    {
        public ControlFreakSkillInfo()
        {
        }

        public ControlFreakSkillInfo(SkillInfo copyFrom, float increaseDamagePercent)
            : base(copyFrom)
        {
            IncreaseDamagePercent = increaseDamagePercent;
        }

        public float IncreaseDamagePercent { get; set; }
    }
}