using System;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class MeleeWeaponStatsConfig : WeaponStatsConfig
    {
        public MeleeWeaponStatsConfig()
        {
        }

        protected MeleeWeaponStatsConfig(WeaponStatsConfig copyFrom, float hitZoneWidth, float hitZoneHeight)
            : base(copyFrom)
        {
            HitZoneWidth = hitZoneWidth;
            HitZoneHeight = hitZoneHeight;
        }

        /// <summary>
        /// Gets or sets the zone width for hit.
        /// </summary>
        public float HitZoneWidth { get; set; }

        /// <summary>
        /// Gets or sets the zone height for hit.
        /// </summary>
        public float HitZoneHeight { get; set; }

        public override WeaponStatsConfig Sum(WeaponStatsConfig add)
        {
            if (add is MeleeWeaponStatsConfig typedAdd)
            {
                return new MeleeWeaponStatsConfig(
                    base.Sum(add),
                    HitZoneWidth + typedAdd.HitZoneWidth,
                    HitZoneHeight + typedAdd.HitZoneHeight);
            }

            throw new InvalidOperationException($"Only instance of type {nameof(MeleeWeaponStatsConfig)} can be added.");
        }
    }
}