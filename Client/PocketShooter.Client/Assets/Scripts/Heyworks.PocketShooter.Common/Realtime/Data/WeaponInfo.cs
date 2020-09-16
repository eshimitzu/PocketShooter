namespace Heyworks.PocketShooter.Realtime.Data
{
    public class WeaponInfo
    {
        public WeaponInfo()
        {
        }

        public WeaponInfo(WeaponInfo copyFrom)
        {
            Name = copyFrom.Name;
            AttackInterval = copyFrom.AttackInterval;
            ReloadTime = copyFrom.ReloadTime;
            ClipSize = copyFrom.ClipSize;
            Dispersion = copyFrom.Dispersion;
            MaxRange = copyFrom.MaxRange;
            CriticalMultiplier = copyFrom.CriticalMultiplier;
            Damage = copyFrom.Damage;
            FractionDispersion = copyFrom.FractionDispersion;
            Fraction = copyFrom.Fraction;
            AutoAim = copyFrom.AutoAim;
        }

        /// <summary>
        /// Gets or sets the weapon name.
        /// </summary>
        public WeaponName Name { get; set; }

        /// <summary>
        /// Gets or sets the weapon attack interval.
        /// </summary>
        public int AttackInterval { get; set; }

        /// <summary>
        /// Gets or sets the reload time.
        /// </summary>
        public int ReloadTime { get; set; }

        /// <summary>
        /// Gets or sets the size of the clip.
        /// </summary>
        public int ClipSize { get; set; }

        /// <summary>
        /// Gets or sets the dispersion.
        /// </summary>
        public float Dispersion { get; set; }

        /// <summary>
        /// Gets or sets the maximum range.
        /// </summary>
        public float MaxRange { get; set; }

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
        public float FractionDispersion { get; set; }

        /// <summary>
        /// Gets or sets the fraction. Number of pellets in one shot.
        /// </summary>
        public int Fraction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a weapon has an auto aim.
        /// </summary>
        public bool AutoAim { get; set; }
    }
}
