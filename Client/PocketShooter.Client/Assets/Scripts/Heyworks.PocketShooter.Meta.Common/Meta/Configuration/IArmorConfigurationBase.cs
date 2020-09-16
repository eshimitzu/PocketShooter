using System;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IArmorConfigurationBase
    {
        /// <summary>
        /// Gets an armor grade's dependent stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="grade">The armor's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetGradeStats(ArmorName armorName, Grade grade);

        /// <summary>
        /// Gets an armor level's dependent stats.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="level">The armor's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetLevelStats(ArmorName armorName, int level);

        /// <summary>
        /// Gets a max potential power for item.
        /// </summary>
        /// <param name="level">The item's level.</param>
        /// <exception cref="ConfigurationException">If a power is not found.</exception>
        int GetMaxPotentialPower(ArmorName armorName);

        /// <summary>
        /// Gets a max armor's level for provided grade.
        /// </summary>
        /// <param name="grade">The armor's grade.</param>
        int GetMaxLevel(Grade grade);

        /// <summary>
        /// Get's first level for provided grade.
        /// </summary>
        /// <param name="grade">The grade.</param>
        int GetFirstLevel(Grade grade);

        /// <summary>
        /// Gets a price for instant level up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetLevel">The armor's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantLevelUpPrice(ArmorName armorName, int targetLevel);

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetLevel">The armor's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetRegularLevelUpPrice(ArmorName armorName, int targetLevel);

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetLevel">The armor's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        TimeSpan GetRegularLevelUpDuration(ArmorName armorName, int targetLevel);

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="targetGrade">The armor's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantGradeUpPrice(ArmorName armorName, Grade targetGrade);
    }
}
