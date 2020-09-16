namespace Heyworks.PocketShooter.Realtime.Data
{
    public class GrenadeSkillInfo : SkillInfo
    {
        public GrenadeSkillInfo()
        {
        }

        public GrenadeSkillInfo(
            SkillInfo copyFrom,
            float splashDamage,
            float damage,
            float explosionRadius,
            int castingTime,
            int availablePerSpawn)
            : base(copyFrom)
        {
            SplashDamage = splashDamage;
            Damage = damage;
            ExplosionRadius = explosionRadius;
            CastingTime = castingTime;
            AvailablePerSpawn = availablePerSpawn;
        }

        public float SplashDamage { get; set; }

        public float Damage { get; set; }

        public float ExplosionRadius { get; set; }

        public int CastingTime { get; set; }

        /// <summary>
        /// Gets or sets the number of grenade can be used per one spawn.
        /// </summary>
        public int AvailablePerSpawn { get; set; }
    }
}