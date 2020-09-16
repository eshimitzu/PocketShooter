using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class ArmorConfiguration : ArmorConfigurationBase, IArmorConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorConfiguration"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public ArmorConfiguration(ServerGameConfig gameConfig)
            : base(gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #region [IArmorConfiguration Implementation]

        /// <summary>
        /// Gets an armor grade's dependent realtime stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="grade">The armor's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ArmorStatsConfig GetGradeRealtimeStats(ArmorName armorName, Grade grade)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .ArmorGradesConfig
                .SingleOrDefault(item => item.Name == armorName && item.Grade == grade)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find armor realtime stats for armor {armorName} and grade {grade}");
            }

            return statsConfig;
        }

        /// <summary>
        /// Gets an armor level's dependent realtime stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="level">The armor's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ArmorStatsConfig GetLevelRealtimeStats(ArmorName armorName, int level)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .ArmorLevelsConfig
                .SingleOrDefault(item => item.Name == armorName && item.Level == level)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find armor realtime stats for armor {armorName} and level {level}");
            }

            return statsConfig;
        }

        #endregion [IArmorConfiguration Implementation]
    }
}
