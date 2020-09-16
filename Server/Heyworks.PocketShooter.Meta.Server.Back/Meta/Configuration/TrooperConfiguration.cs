using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class TrooperConfiguration : TrooperConfigurationBase, ITrooperConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrooperConfiguration"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public TrooperConfiguration(ServerGameConfig gameConfig)
            : base(gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #region [ITrooperConfiguration Implementation]

        /// <summary>
        /// Gets a trooper grade's dependent realtime stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="grade">The trooper's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public TrooperStatsConfig GetGradeRealtimeStats(TrooperClass trooperClass, Grade grade)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .TrooperGradesConfig
                .SingleOrDefault(item => item.Class == trooperClass && item.Grade == grade)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find trooper realtime stats for trooper {trooperClass} and grade {grade}");
            }

            return statsConfig;
        }

        /// <summary>
        /// Gets a trooper level's dependent realtime stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public TrooperStatsConfig GetLevelRealtimeStats(TrooperClass trooperClass, int level)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .TrooperLevelsConfig
                .SingleOrDefault(item => item.Class == trooperClass && item.Level == level)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find trooper realtime stats for trooper {trooperClass} and level {level}");
            }

            return statsConfig;
        }

        #endregion [ITrooperConfiguration Implementation]
    }
}
