namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class WeaponStatsConfig
    {
        public WeaponStatsConfig()
        {
        }

        protected WeaponStatsConfig(WeaponStatsConfig copyFrom)
        {
            AttackIntervalMs = copyFrom.AttackIntervalMs;
            ClipSize = copyFrom.ClipSize;
            MaxRange = copyFrom.MaxRange;
            ReloadTimeMs = copyFrom.ReloadTimeMs;
            Dispersion = copyFrom.Dispersion;
            CriticalMultiplier = copyFrom.CriticalMultiplier;
            Damage = copyFrom.Damage;
            FractionDispersion = copyFrom.FractionDispersion;
            Fraction = copyFrom.Fraction;
            AutoAim = copyFrom.AutoAim;
        }

        public int AttackIntervalMs { get; set; }

        public int ClipSize { get; set; }

        public int MaxRange { get; set; }

        /// <summary>
        /// Gets or sets the reload time in milliseconds.
        /// </summary>
        public int ReloadTimeMs { get; set; }

        /// <summary>
        /// Gets or sets the dispersion.
        /// </summary>
        /// <value>The dispersion.</value>
        public float Dispersion { get; set; }

        /// <summary>
        /// Gets or sets the critical multiplier.
        /// </summary>
        public float CriticalMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the weapon's damage.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets the fraction dispersion. Spreading bullets in meters for each 100m.
        /// </summary>
        /// <value>The fraction dispersion.</value>
        public float FractionDispersion { get; set; }

        /// <summary>
        /// Gets or sets the fraction. Number of pellets in one shot.
        /// </summary>
        /// <value>The fraction.</value>
        public int Fraction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a weapon has an auto aim.
        /// </summary>
        public bool AutoAim { get; set; }

        public virtual WeaponStatsConfig Sum(WeaponStatsConfig add)
        {
            return new WeaponStatsConfig
            {
                AttackIntervalMs = AttackIntervalMs + add.AttackIntervalMs,
                ClipSize = ClipSize + add.ClipSize,
                MaxRange = MaxRange + add.MaxRange,
                ReloadTimeMs = ReloadTimeMs + add.ReloadTimeMs,
                Dispersion = Dispersion + add.Dispersion,
                CriticalMultiplier = CriticalMultiplier + add.CriticalMultiplier,
                Damage = Damage + add.Damage,
                FractionDispersion = FractionDispersion + add.FractionDispersion,
                Fraction = Fraction + add.Fraction,
                AutoAim = AutoAim | add.AutoAim,
            };
        }
    }
}
