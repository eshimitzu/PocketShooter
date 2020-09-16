namespace Heyworks.PocketShooter.Realtime.Data
{
    public class SkillInfo
    {
        public SkillInfo()
        {
        }

        public SkillInfo(SkillInfo copyFrom)
        {
            Name = copyFrom.Name;
            CooldownTime = copyFrom.CooldownTime;
            ActiveTime = copyFrom.ActiveTime;
        }

        public SkillName Name { get; set; }

        public int CooldownTime { get; set; }

        public int ActiveTime { get; set; }
    }
}
