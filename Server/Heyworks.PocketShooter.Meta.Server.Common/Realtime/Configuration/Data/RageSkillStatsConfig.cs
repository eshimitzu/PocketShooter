namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class RageSkillStatsConfig : SkillStatsConfig
    {
        public int IncreaseDamagePercent { get; set; }

        public int DecreaseDamagePercent { get; set; }

        public int IncreaseDamageIntervalMs { get; set; }

        public int DecreaseDamageIntervalMs { get; set; }
    }
}