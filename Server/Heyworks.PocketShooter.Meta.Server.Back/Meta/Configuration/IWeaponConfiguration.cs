using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IWeaponConfiguration : IWeaponConfigurationBase
    {
        /// <summary>
        /// Gets a weapon grade's dependent realtime stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="grade">The weapon's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        WeaponStatsConfig GetGradeRealtimeStats(WeaponName weaponName, Grade grade);

        /// <summary>
        /// Gets a weapon level's dependent realtime stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="level">The weapon's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        WeaponStatsConfig GetLevelRealtimeStats(WeaponName weaponName, int level);
    }
}
