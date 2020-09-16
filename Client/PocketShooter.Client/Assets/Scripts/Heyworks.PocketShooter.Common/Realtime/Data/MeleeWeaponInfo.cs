namespace Heyworks.PocketShooter.Realtime.Data
{
    public class MeleeWeaponInfo : WeaponInfo
    {
        public MeleeWeaponInfo()
        {
        }

        public MeleeWeaponInfo(WeaponInfo copyFrom, float hitZoneWidth, float hitZoneHeight)
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
    }
}
