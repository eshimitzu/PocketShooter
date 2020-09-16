using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class HelmetConfiguration : HelmetConfigurationBase, IHelmetConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelmetConfiguration"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public HelmetConfiguration(ServerGameConfig gameConfig)
            : base(gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #region [IHelmetConfiguration Implementation]

        /// <summary>
        /// Gets a helmet grade's dependent realtime stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="grade">The helmet's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public HelmetStatsConfig GetGradeRealtimeStats(HelmetName helmetName, Grade grade)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .HelmetGradesConfig
                .SingleOrDefault(item => item.Name == helmetName && item.Grade == grade)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find helmet realtime stats for helmet {helmetName} and grade {grade}");
            }

            return statsConfig;
        }

        /// <summary>
        /// Gets a helmet level's dependent realtime stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="level">The helmet's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public HelmetStatsConfig GetLevelRealtimeStats(HelmetName helmetName, int level)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .HelmetLevelsConfig
                .SingleOrDefault(item => item.Name == helmetName && item.Level == level)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find helmet realtime stats for helmet {helmetName} and level {level}");
            }

            return statsConfig;
        }

        #endregion [IHelmetConfiguration Implementation]
    }
}
