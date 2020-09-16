namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class WeaponGradeConfig
    {
        /// <summary>
        /// Gets or sets the weapon name.
        /// </summary>
        public WeaponName Name { get; set; }

        /// <summary>
        /// Gets or sets the weapon grade.
        /// </summary>
        public Grade Grade { get; set; }

        public WeaponStatsConfig Stats { get; set; }
    }
}