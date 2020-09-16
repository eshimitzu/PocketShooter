using System;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class WarmingUpWeaponStatsConfig : WeaponStatsConfig
    {
        public WarmingUpWeaponStatsConfig()
        {
        }

        protected WarmingUpWeaponStatsConfig(WeaponStatsConfig copyFrom, float warmingSpeed, float coolingSpeed, bool resetProgressOnShot)
            : base(copyFrom)
        {
            WarmingSpeed = warmingSpeed;
            CoolingSpeed = coolingSpeed;
            ResetProgressOnShot = resetProgressOnShot;
        }

        /// <summary>
        /// Gets or sets warming speed.
        /// </summary>
        public float WarmingSpeed { get; set; }

        /// <summary>
        /// Gets or sets cooling speed.
        /// </summary>
        public float CoolingSpeed { get; set; }

        /// <summary>
        /// Gets or sets if progress should be reset on shot.
        /// </summary>
        public bool ResetProgressOnShot { get; set; }

        public override WeaponStatsConfig Sum(WeaponStatsConfig add)
        {
            if (add is WarmingUpWeaponStatsConfig typedAdd)
            {
                return new WarmingUpWeaponStatsConfig(
                    base.Sum(add),
                    WarmingSpeed + typedAdd.WarmingSpeed,
                    CoolingSpeed + typedAdd.CoolingSpeed,
                    ResetProgressOnShot | typedAdd.ResetProgressOnShot);
            }

            throw new InvalidOperationException($"Only instance of type {nameof(WarmingUpWeaponStatsConfig)} can be added.");
        }
    }
}
