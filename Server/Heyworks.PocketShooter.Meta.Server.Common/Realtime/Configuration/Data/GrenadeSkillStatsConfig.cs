namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class GrenadeSkillStatsConfig : SkillStatsConfig
    {
        public int CastingTimeMs { get; set; }

        public float SplashDamage { get; set; }

        public float Damage { get; set; }

        public float ExplosionRadius { get; set; }

        /// <summary>
        /// Gets the number of grenade can be used per one spawn.
        /// </summary>
        public int AvailablePerSpawn { get; set; }
    }
}