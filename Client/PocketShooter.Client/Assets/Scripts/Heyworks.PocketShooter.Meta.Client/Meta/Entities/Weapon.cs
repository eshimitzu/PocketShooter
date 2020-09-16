using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Weapon : WeaponBase
    {
        private readonly IWeaponConfigurationBase weaponConfiguration;

        public Weapon(WeaponState weaponState, IWeaponConfigurationBase weaponConfiguration)
            : base(weaponState, weaponConfiguration)
        {
            this.weaponConfiguration = weaponConfiguration;
        }

        public ItemStats NextLevelStats =>
            !IsMaxLevel
                ? ItemStats.Sum(
                    weaponConfiguration.GetGradeStats(Name, Grade),
                    weaponConfiguration.GetLevelStats(Name, Level + 1))
                : Stats;


        public ItemStats NextGradeStats =>
            !Grade.IsMax()
                ? ItemStats.Sum(
                    weaponConfiguration.GetGradeStats(Name, Grade + 1),
                    weaponConfiguration.GetLevelStats(Name, Level))
                : Stats;

        public int NextGradeMaxLevel => Grade.IsMax() ? MaxLevel : weaponConfiguration.GetMaxLevel(Grade + 1);
    }
}
