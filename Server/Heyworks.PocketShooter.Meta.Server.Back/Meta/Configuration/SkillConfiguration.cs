using System;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Represents a skill configuration.
    /// </summary>
    public class SkillConfiguration : ISkillConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillConfiguration"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public SkillConfiguration(ServerGameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #region [ISkillConfiguration Implementation]

        /// <summary>
        /// Gets a skill realtime stats.
        /// </summary>
        /// <param name="skillName">The skill name.</param>
        /// <param name="grade">The skill's grade.</param>
        /// <exception cref="InvalidOperationException">If a stats is not found.</exception>
        public SkillStatsConfig GetRealtimeStats(SkillName skillName, Grade grade)
        {
            var statsConfig =
                gameConfig
                .RealtimeConfig
                .SkillGradesConfig
                .SingleOrDefault(item => item.Name == skillName && item.Grade == grade)
                ?.Stats;

            if (statsConfig == null)
            {
                throw new ConfigurationException($"Can't find skill realtime stats for skill {skillName} and grade {grade}");
            }

            return statsConfig;
        }

        #endregion [ISkillConfiguration Implementation]
    }
}
