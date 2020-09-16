using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Configuration component that contains trooper related configuration methods shared between client and server.
    /// </summary>
    public class TrooperConfigurationBase : ITrooperConfigurationBase
    {
        #region [Private Members]

        private readonly GameConfig gameConfig;

        #endregion [Private Members]

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="TrooperConfigurationBase"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public TrooperConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #endregion [Constructors and initialization]

        #region [Implementation of ITrooperConfigurationBase]

        /// <summary>
        /// Gets a trooper grade's dependent stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="grade">The trooper's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetGradeStats(TrooperClass trooperClass, Grade grade) =>
            gameConfig.GetTrooperGradeConfig(trooperClass, grade).Stats;

        /// <summary>
        /// Gets a trooper level's dependent stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetLevelStats(TrooperClass trooperClass, int level) =>
            gameConfig.GetTrooperLevelConfig(trooperClass, level).Stats;

        /// <summary>
        /// Gets a max potential power for the trooper.
        /// </summary>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a power is not found.</exception>
        public int GetMaxPotentialPower(TrooperClass trooperClass)
        {
            var maxGrade = CommonConfiguration.MaxGrade;
            var maxLevel = GetMaxLevel(maxGrade);

            return ItemStats
                .Sum(GetGradeStats(trooperClass, maxGrade), GetLevelStats(trooperClass, maxLevel))
                .Power;
        }

        /// <summary>
        /// Gets a max trooper's level for provided grade.
        /// </summary>
        /// <param name="grade">The trooper's grade.</param>
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
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetLevel">The trooper's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantLevelUpPrice(TrooperClass trooperClass, int targetLevel) =>
            gameConfig.GetTrooperLevelConfig(trooperClass, targetLevel).InstantPrice;

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetLevel">The trooper's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetRegularLevelUpPrice(TrooperClass trooperClass, int targetLevel) =>
            gameConfig.GetTrooperLevelConfig(trooperClass, targetLevel).RegularPrice;

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="trooperClass">The troopers's class.</param>
        /// <param name="targetLevel">The trooper's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        public TimeSpan GetRegularLevelUpDuration(TrooperClass trooperClass, int targetLevel) =>
            gameConfig.GetTrooperLevelConfig(trooperClass, targetLevel).RegularDuration;

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetGrade">The trooper's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantGradeUpPrice(TrooperClass trooperClass, Grade targetGrade) =>
            gameConfig.GetTrooperGradeConfig(trooperClass, targetGrade).InstantPrice;

        /// <summary>
        /// Gets a trooper's skills
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <exception cref="InvalidOperationException">If a skills is not found.</exception>
        public IReadOnlyList<SkillName> GetSkills(TrooperClass trooperClass)
        {
            var skillsConfig =
                gameConfig
                .TrooperClassesConfig
                .SingleOrDefault(item => item.Class == trooperClass)
                ?.Skills;

            if (skillsConfig == null)
            {
                throw new ConfigurationException($"Can't find skills for trooper {trooperClass}");
            }

            return skillsConfig.ToList();
        }

        #endregion [Implementation of ITrooperConfigurationBase]
    }
}
