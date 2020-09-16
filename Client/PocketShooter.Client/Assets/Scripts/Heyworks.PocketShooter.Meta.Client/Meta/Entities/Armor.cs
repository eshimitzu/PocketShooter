using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Armor : ArmorBase
    {
        private readonly IArmorConfigurationBase armorConfiguration;

        public Armor(ArmorState armorState, IArmorConfigurationBase armorConfiguration)
            : base(armorState, armorConfiguration)
        {
            this.armorConfiguration = armorConfiguration;
        }

        public ItemStats NextLevelStats =>
            !IsMaxLevel
                ? ItemStats.Sum(
                    armorConfiguration.GetGradeStats(Name, Grade),
                    armorConfiguration.GetLevelStats(Name, Level + 1))
                : Stats;


        public ItemStats NextGradeStats =>
            !Grade.IsMax()
                ? ItemStats.Sum(
                    armorConfiguration.GetGradeStats(Name, Grade + 1),
                    armorConfiguration.GetLevelStats(Name, Level))
                : Stats;

        public int NextGradeMaxLevel => Grade.IsMax() ? MaxLevel : armorConfiguration.GetMaxLevel(Grade + 1);
    }
}