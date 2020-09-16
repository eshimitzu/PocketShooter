using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IArmorConfiguration : IArmorConfigurationBase
    {
        /// <summary>
        /// Gets an armor grade's dependent realtime stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="grade">The armor's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ArmorStatsConfig GetGradeRealtimeStats(ArmorName armorName, Grade grade);

        /// <summary>
        /// Gets an armor level's dependent realtime stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="level">The armor's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ArmorStatsConfig GetLevelRealtimeStats(ArmorName armorName, int level);
    }
}
