namespace Heyworks.PocketShooter.Realtime.Data
{
    public class StealthDashSkillInfo : SkillInfo
    {
        public StealthDashSkillInfo()
        {
        }

        public StealthDashSkillInfo(
            SkillInfo copyFrom,
            int castingTime,
            int dashTime,
            float length,
            float speed)
            : base(copyFrom)
        {
            CastingTime = castingTime;
            DashTime = dashTime;
            Length = length;
            Speed = speed;
        }

        public int CastingTime { get; set; }

        public int DashTime { get; set; }

        public float Length { get; set; }

        public float Speed { get; set; }
    }
}