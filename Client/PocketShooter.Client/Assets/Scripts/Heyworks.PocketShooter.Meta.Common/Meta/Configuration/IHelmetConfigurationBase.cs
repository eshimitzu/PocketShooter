using Heyworks.PocketShooter.Meta.Entities;
using System;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IHelmetConfigurationBase
    {
        /// <summary>
        /// Gets a helmet grade's dependent stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="grade">The helmet's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetGradeStats(HelmetName helmetName, Grade grade);

        /// <summary>
        /// Gets a helmet level's dependent stats.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="level">The helmet's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetLevelStats(HelmetName helmetName, int level);

        /// <summary>
        /// Gets a max potential power for item.
        /// </summary>
        /// <param name="level">The item's level.</param>
        /// <exception cref="ConfigurationException">If a power is not found.</exception>
        int GetMaxPotentialPower(HelmetName helmetName);

        /// <summary>
        /// Gets a max helmet's level for provided grade.
        /// </summary>
        /// <param name="grade">The helmet's grade.</param>
        int GetMaxLevel(Grade grade);

        /// <summary>
        /// Get's first level for provided grade.
        /// </summary>
        /// <param name="grade">The grade.</param>
        int GetFirstLevel(Grade grade);

        /// <summary>
        /// Gets a price for instant level up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetLevel">The helmet's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantLevelUpPrice(HelmetName helmetName, int targetLevel);

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetLevel">The helmet's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetRegularLevelUpPrice(HelmetName helmetName, int targetLevel);

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetLevel">The helmet's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        TimeSpan GetRegularLevelUpDuration(HelmetName helmetName, int targetLevel);

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="targetGrade">The helmet's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantGradeUpPrice(HelmetName helmetName, Grade targetGrade);
    }
}