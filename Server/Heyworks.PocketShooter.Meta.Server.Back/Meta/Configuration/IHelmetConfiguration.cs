using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IHelmetConfiguration : IHelmetConfigurationBase
    {
        /// <summary>
        /// Gets a helmet grade's dependent realtime stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="grade">The helmet's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        HelmetStatsConfig GetGradeRealtimeStats(HelmetName helmetName, Grade grade);

        /// <summary>
        /// Gets a helmet level's dependent realtime stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="level">The helmet's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        HelmetStatsConfig GetLevelRealtimeStats(HelmetName helmetName, int level);
    }
}
