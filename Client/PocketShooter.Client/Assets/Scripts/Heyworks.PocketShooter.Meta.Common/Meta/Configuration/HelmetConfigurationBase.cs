using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using System;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Configuration component that contains helmet related configuration methods shared between client and server.
    /// </summary>
    public class HelmetConfigurationBase : IHelmetConfigurationBase
    {
        #region [Private Members]

        private readonly GameConfig gameConfig;

        #endregion [Private Members]

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="HelmetConfigurationBase"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public HelmetConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #endregion [Constructors and initialization]

        #region [Implementation of IHelmetConfigurationBase]

        /// <summary>
        /// Gets a helmet grade's dependent stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="grade">The helmet's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetGradeStats(HelmetName helmetName, Grade grade) =>
            gameConfig.GetHelmetGradeConfig(helmetName, grade).Stats;

        /// <summary>
        /// Gets a helmet level's dependent stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="level">The helmet's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetLevelStats(HelmetName helmetName, int level) =>
            gameConfig.GetHelmetLevelConfig(helmetName, level).Stats;

        /// <inheritdoc cref="IHelmetConfigurationBase"/>
        public int GetMaxPotentialPower(HelmetName helmetName)
        {
            var maxGrade = CommonConfiguration.MaxGrade;
            var maxLevel = GetMaxLevel(maxGrade);

            return ItemStats
                .Sum(GetGradeStats(helmetName, maxGrade), GetLevelStats(helmetName, maxLevel))
                .Power;
        }

        /// <summary>
        /// Gets a max helmet's level for provided grade.
        /// </summary>
        /// <param name="grade">The helmet's grade.</param>
        public int GetMaxLevel(Grade grade) => gameConfig.GradesConfig.GetMaxLevel(grade);

        /// <summary>
        /// Get's first level for provided grade.
        /// </summary>
        /// <param name="grade">The grade.</param>
        public int GetFirstLevel(Grade grade)
        {
            if (grade == Grade.Common)
            {
                return 1;
            }

            return GetMaxLevel(grade - 1);
        }

        /// <summary>
        /// Gets a price for instant level up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetLevel">The helmet's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantLevelUpPrice(HelmetName helmetName, int targetLevel) =>
            gameConfig.GetHelmetLevelConfig(helmetName, targetLevel).InstantPrice;

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetLevel">The helmet's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetRegularLevelUpPrice(HelmetName helmetName, int targetLevel) =>
            gameConfig.GetHelmetLevelConfig(helmetName, targetLevel).RegularPrice;

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetLevel">The helmet's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        public TimeSpan GetRegularLevelUpDuration(HelmetName helmetName, int targetLevel) =>
            gameConfig.GetHelmetLevelConfig(helmetName, targetLevel).RegularDuration;

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetGrade">The helmet's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantGradeUpPrice(HelmetName helmetName, Grade targetGrade) =>
            gameConfig.GetHelmetGradeConfig(helmetName, targetGrade).InstantPrice;

        #endregion [Implementation of IHelmetConfigurationBase]
    }
}
