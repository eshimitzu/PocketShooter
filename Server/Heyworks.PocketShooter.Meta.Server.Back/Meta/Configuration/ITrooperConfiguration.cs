using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface ITrooperConfiguration : ITrooperConfigurationBase
    {
        /// <summary>
        /// Gets a trooper grade's dependent realtime stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="grade">The trooper's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        TrooperStatsConfig GetGradeRealtimeStats(TrooperClass trooperClass, Grade grade);

        /// <summary>
        /// Gets a trooper level's dependent realtime stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        TrooperStatsConfig GetLevelRealtimeStats(TrooperClass trooperClass, int level);
    }
}
