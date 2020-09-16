using Heyworks.PocketShooter.Meta.Entities;
using System;
using System.Collections.Generic;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface ITrooperConfigurationBase
    {
        /// <summary>
        /// Gets a trooper grade's dependent stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="grade">The trooper's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetGradeStats(TrooperClass trooperClass, Grade grade);

        /// <summary>
        /// Gets a trooper level's dependent stats.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetLevelStats(TrooperClass trooperClass, int level);

        /// <summary>
        /// Gets a max potential power for the trooper.
        /// </summary>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a power is not found.</exception>
        int GetMaxPotentialPower(TrooperClass trooperClass);

        /// <summary>
        /// Gets a max trooper's level for provided grade.
        /// </summary>
        /// <param name="grade">The trooper's grade.</param>
        int GetMaxLevel(Grade grade);

        /// <summary>
        /// Get's first level for provided grade.
        /// </summary>
        /// <param name="grade">The trooper's grade.</param>
        int GetFirstLevel(Grade grade);
        
        /// <summary>
        /// Gets a price for instant level up.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetLevel">The trooper's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantLevelUpPrice(TrooperClass trooperClass, int targetLevel);

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetLevel">The trooper's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetRegularLevelUpPrice(TrooperClass trooperClass, int targetLevel);

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetLevel">The trooper's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        TimeSpan GetRegularLevelUpDuration(TrooperClass trooperClass, int targetLevel);

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="targetGrade">The trooper's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantGradeUpPrice(TrooperClass trooperClass, Grade targetGrade);

        /// <summary>
        /// Gets a trooper's skills
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <exception cref="System.InvalidOperationException">If a skills is not found.</exception>
        IReadOnlyList<SkillName> GetSkills(TrooperClass trooperClass);
    }
}
