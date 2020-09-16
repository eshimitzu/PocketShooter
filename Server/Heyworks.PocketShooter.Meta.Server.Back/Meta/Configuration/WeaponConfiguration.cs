using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class WeaponConfiguration : WeaponConfigurationBase, IWeaponConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponConfiguration"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public WeaponConfiguration(ServerGameConfig gameConfig)
            : base(gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #region [IWeaponConfiguration Implementation]

        /// <summary>
        /// Gets a weapon grade's dependent realtime stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="grade">The weapon's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public WeaponStatsConfig GetGradeRealtimeStats(WeaponName weaponName, Grade grade)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .WeaponGradesConfig
                .SingleOrDefault(item => item.Name == weaponName && item.Grade == grade)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find weapon realtime stats for weapon {weaponName} and grade {grade}");
            }

            return statsConfig;
        }

        /// <summary>
        /// Gets a weapon level's dependent realtime stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="level">The weapon's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public WeaponStatsConfig GetLevelRealtimeStats(WeaponName weaponName, int level)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .WeaponLevelsConfig
                .SingleOrDefault(item => item.Name == weaponName && item.Level == level)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find weapon realtime stats for weapon {weaponName} and level {level}");
            }

            return statsConfig;
        }

        #endregion [IWeaponConfiguration Implementation]
    }
}
