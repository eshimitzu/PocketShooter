namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class ArmorGradeConfig
    {
        /// <summary>
        /// Gets or sets the armor name.
        /// </summary>
        public ArmorName Name { get; set; }

        /// <summary>
        /// Gets or sets the armor grade.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// Gets or sets an armor stats.
        /// </summary>
        public ArmorStatsConfig Stats { get; set; }
    }
}