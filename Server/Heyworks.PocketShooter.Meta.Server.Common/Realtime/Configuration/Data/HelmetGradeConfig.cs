namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class HelmetGradeConfig
    {
        /// <summary>
        /// Gets or sets the helmet name.
        /// </summary>
        public HelmetName Name { get; set; }

        /// <summary>
        /// Gets or sets the helmet grade.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// Gets or sets a helmet stats.
        /// </summary>
        public HelmetStatsConfig Stats { get; set; }
    }
}