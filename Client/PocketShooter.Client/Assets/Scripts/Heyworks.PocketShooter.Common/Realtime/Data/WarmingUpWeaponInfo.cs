namespace Heyworks.PocketShooter.Realtime.Data
{
    public class WarmingUpWeaponInfo : WeaponInfo
    {
        public WarmingUpWeaponInfo()
        {
        }

        public WarmingUpWeaponInfo(WeaponInfo copyFrom, float warmingSpeed, float coolingSpeed, bool resetProgressOnShot)
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
    }
}
