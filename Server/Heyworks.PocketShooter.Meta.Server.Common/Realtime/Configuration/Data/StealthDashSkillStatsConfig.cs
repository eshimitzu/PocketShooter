using System;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class StealthDashSkillStatsConfig : SkillStatsConfig
    {
        public int CastingTimeMs { get; set; }

        public float Length { get; set; }

        public float Speed { get; set; }

        public int DashTimeMs => (int)Math.Round(1000f * Length / Speed);
    }
}