namespace Heyworks.PocketShooter.Realtime.Data
{
    public class StealthSprintSkillInfo : SprintSkillInfo
    {
        public StealthSprintSkillInfo()
        {
        }

        public StealthSprintSkillInfo(SkillInfo copyFrom, float speedMultiplier, int stealthActiveTime)
            : base(copyFrom, speedMultiplier)
        {
            StealthActiveTime = stealthActiveTime;
        }

        public int StealthActiveTime { get; set; }
    }
}