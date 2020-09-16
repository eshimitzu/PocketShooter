using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using System;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Configuration component that contains weapon related configuration methods shared between client and server.
    /// </summary>
    public class WeaponConfigurationBase : IWeaponConfigurationBase
    {
        #region [Private Members]

        private readonly GameConfig gameConfig;

        #endregion [Private Members]

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponConfigurationBase"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public WeaponConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #endregion [Constructors and initialization]

        #region [Implementation of IWeaponConfigurationBase]

        /// <summary>
        /// Gets a weapon grade's dependent stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="grade">The weapon's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetGradeStats(WeaponName weaponName, Grade grade) =>
            gameConfig.GetWeaponGradeConfig(weaponName, grade).Stats;

        /// <summary>
        /// Gets a weapon level's dependent stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="level">The weapon's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        public ItemStats GetLevelStats(WeaponName weaponName, int level) =>
            gameConfig.GetWeaponLevelConfig(weaponName, level).Stats;

        /// <inheritdoc cref="IWeaponConfigurationBase"/>
        public int GetMaxPotentialPower(WeaponName weaponName)
        {
            var maxGrade = CommonConfiguration.MaxGrade;
            var maxLevel = GetMaxLevel(maxGrade);

            return ItemStats
                .Sum(GetGradeStats(weaponName, maxGrade), GetLevelStats(weaponName, maxLevel))
                .Power;
        }

        /// <summary>
        /// Gets a max weapon's level for provided grade.
        /// </summary>
        /// <param name="grade">The weapon's grade.</param>
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
        /// <param name="trooperClass">The weapon's name.</param>
        /// <param name="targetLevel">The weapon's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantLevelUpPrice(WeaponName weaponName, int targetLevel) =>
            gameConfig.GetWeaponLevelConfig(weaponName, targetLevel).InstantPrice;

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="trooperClass">The weapon's name.</param>
        /// <param name="targetLevel">The weapon's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetRegularLevelUpPrice(WeaponName weaponName, int targetLevel) =>
            gameConfig.GetWeaponLevelConfig(weaponName, targetLevel).RegularPrice;

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="targetLevel">The weapon's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        public TimeSpan GetRegularLevelUpDuration(WeaponName weaponName, int targetLevel) =>
            gameConfig.GetWeaponLevelConfig(weaponName, targetLevel).RegularDuration;

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="trooperClass">The weapon's name.</param>
        /// <param name="targetGrade">The weapon's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        public Price GetInstantGradeUpPrice(WeaponName weaponName, Grade targetGrade) =>
            gameConfig.GetWeaponGradeConfig(weaponName, targetGrade).InstantPrice;

        #endregion [Implementation of IWeaponConfigurationBase]
    }
}
