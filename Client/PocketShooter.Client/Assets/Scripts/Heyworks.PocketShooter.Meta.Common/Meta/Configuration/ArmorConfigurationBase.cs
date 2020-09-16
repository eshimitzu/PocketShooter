using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using System;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Configuration component that contains armor related configuration methods shared between client and server.
    /// </summary>
    public class ArmorConfigurationBase : IArmorConfigurationBase
    {
        #region [Private Members]

        private readonly GameConfig gameConfig;

        #endregion [Private Members]

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorConfigurationBase"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public ArmorConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #endregion [Constructors and initialization]

        #region [Implementation of IArmorConfigurationBase]

        /// <summary>
        /// Gets an armor grade's dependent stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="grade">The armor's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetGradeStats(ArmorName armorName, Grade grade) =>
            gameConfig.GetArmorGradeConfig(armorName, grade).Stats;

        /// <summary>
        /// Gets a armor level's dependent stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="level">The armor's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetLevelStats(ArmorName armorName, int level) =>
            gameConfig.GetArmorLevelConfig(armorName, level).Stats;

        /// <inheritdoc cref="IArmorConfigurationBase"/>
        public int GetMaxPotentialPower(ArmorName armorName)
        {
            var maxGrade = CommonConfiguration.MaxGrade;
            var maxLevel = GetMaxLevel(maxGrade);

            return ItemStats
                .Sum(GetGradeStats(armorName, maxGrade), GetLevelStats(armorName, maxLevel))
                .Power;
        }

        /// <summary>
        /// Gets a max armor's level for provided grade.
        /// </summary>
        /// <param name="grade">The armor's grade.</param>
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
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetLevel">The armor's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantLevelUpPrice(ArmorName armorName, int targetLevel) =>
            gameConfig.GetArmorLevelConfig(armorName, targetLevel).InstantPrice;

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetLevel">The armor's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetRegularLevelUpPrice(ArmorName armorName, int targetLevel) =>
            gameConfig.GetArmorLevelConfig(armorName, targetLevel).RegularPrice;

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetLevel">The armor's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        public TimeSpan GetRegularLevelUpDuration(ArmorName armorName, int targetLevel) =>
            gameConfig.GetArmorLevelConfig(armorName, targetLevel).RegularDuration;

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetGrade">The armor's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantGradeUpPrice(ArmorName armorName, Grade targetGrade) =>
            gameConfig.GetArmorGradeConfig(armorName, targetGrade).InstantPrice;

        #endregion [Implementation of IArmorConfigurationBase]
    }
}
