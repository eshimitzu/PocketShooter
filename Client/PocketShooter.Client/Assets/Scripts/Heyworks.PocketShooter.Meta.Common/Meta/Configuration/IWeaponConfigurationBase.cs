using Heyworks.PocketShooter.Meta.Entities;
using System;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IWeaponConfigurationBase
    {
        /// <summary>
        /// Gets a weapon grade's dependent stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="grade">The weapon's grade.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetGradeStats(WeaponName weaponName, Grade grade);

        /// <summary>
        /// Gets a weapon level's dependent stats.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="level">The weapon's level.</param>
        /// <exception cref="ConfigurationException">If a stats is not found.</exception>
        ItemStats GetLevelStats(WeaponName weaponName, int level);

        /// <summary>
        /// Gets a max potential power for item.
        /// </summary>
        /// <param name="level">The item's level.</param>
        /// <exception cref="ConfigurationException">If a power is not found.</exception>
        int GetMaxPotentialPower(WeaponName weaponName);

        /// <summary>
        /// Gets a max weapon's level for provided grade.
        /// </summary>
        /// <param name="grade">The weapon's grade.</param>
        int GetMaxLevel(Grade grade);

        /// <summary>
        /// Get's first level for provided grade.
        /// </summary>
        /// <param name="grade">The grade.</param>
        int GetFirstLevel(Grade grade);

        /// <summary>
        /// Gets a price for instant level up.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="targetLevel">The weapon's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantLevelUpPrice(WeaponName weaponName, int targetLevel);

        /// <summary>
        /// Gets a price for regular level up.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="targetLevel">The weapon's target level.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetRegularLevelUpPrice(WeaponName weaponName, int targetLevel);

        /// <summary>
        /// Gets a duration for regular level up.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="targetLevel">The weapon's target level.</param>
        /// <exception cref="ConfigurationException">If a duration is not found.</exception>
        TimeSpan GetRegularLevelUpDuration(WeaponName weaponName, int targetLevel);

        /// <summary>
        /// Gets a price for instant grade up.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="targetGrade">The weapon's target grade.</param>
        /// <exception cref="ConfigurationException">If a price is not found.</exception>
        Price GetInstantGradeUpPrice(WeaponName weaponName, Grade targetGrade);
    }
}
