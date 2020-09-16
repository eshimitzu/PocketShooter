namespace Heyworks.PocketShooter.Realtime.Data
{
    public class RegenerationOnKillSkillInfo : SkillInfo
    {
        public RegenerationOnKillSkillInfo()
        {
        }

        public RegenerationOnKillSkillInfo(SkillInfo copyFrom, float regenerationPercent)
            : base(copyFrom)
        {
            RegenerationPercent = regenerationPercent;
        }

        public float RegenerationPercent { get; set; }
    }
}