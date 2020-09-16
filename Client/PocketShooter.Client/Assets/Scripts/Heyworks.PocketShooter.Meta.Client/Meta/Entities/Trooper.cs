using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using System.Linq;
using Heyworks.PocketShooter.Meta.Runnables;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Trooper : TrooperBase
    {
        private readonly ArmyState armyState;
        private readonly TrooperState trooperState;
        private readonly ITrooperConfigurationBase trooperConfiguration;
        private readonly IWeaponConfigurationBase weaponConfiguration;
        private readonly IHelmetConfigurationBase helmetConfiguration;
        private readonly IArmorConfigurationBase armorConfiguration;

        public Trooper(
            ArmyState armyState,
            TrooperState trooperState,
            ITrooperConfigurationBase trooperConfiguration,
            IWeaponConfigurationBase weaponConfiguration,
            IHelmetConfigurationBase helmetConfiguration,
            IArmorConfigurationBase armorConfiguration)
            : base(trooperState, trooperConfiguration)
        {
            this.armyState = armyState;
            this.trooperState = trooperState;
            this.trooperConfiguration = trooperConfiguration;
            this.weaponConfiguration = weaponConfiguration;
            this.helmetConfiguration = helmetConfiguration;
            this.armorConfiguration = armorConfiguration;
        }

        public Weapon CurrentWeapon
        {
            get
            {
                var weaponState = armyState.Weapons.First(item => item.Name == trooperState.CurrentWeapon);

                return new Weapon(weaponState, weaponConfiguration);
            }
        }

        public Helmet CurrentHelmet
        {
            get
            {
                var helmetState = armyState.Helmets.First(item => item.Name == trooperState.CurrentHelmet);

                return new Helmet(helmetState, helmetConfiguration);
            }
        }

        public Armor CurrentArmor
        {
            get
            {
                var armorState = armyState.Armors.First(item => item.Name == trooperState.CurrentArmor);

                return new Armor(armorState, armorConfiguration);
            }
        }

        public ItemStats NextLevelStats =>
            !IsMaxLevel
            ? ItemStats.Sum(
                trooperConfiguration.GetGradeStats(Class, Grade),
                trooperConfiguration.GetLevelStats(Class, Level + 1))
            : Stats;


        public ItemStats NextGradeStats =>
            !Grade.IsMax()
            ? ItemStats.Sum(
                trooperConfiguration.GetGradeStats(Class, Grade + 1),
                trooperConfiguration.GetLevelStats(Class, Level))
            : Stats;

        public int NextGradeMaxLevel => Grade.IsMax() ? MaxLevel : trooperConfiguration.GetMaxLevel(Grade + 1);
    }
}
