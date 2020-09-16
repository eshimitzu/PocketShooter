using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Meta.Entities
{
    // TODO: a.dezhurko Bad inheritance.
    // TODO: v.shimkovich need only stats+config from base class, create separate one
    public class RosterArmorProduct : ArmorBase
    {
        private readonly RosterProduct rosterProduct;
        private readonly IArmorConfigurationBase armorConfiguration;

        public RosterArmorProduct(
            IArmorConfigurationBase armorConfiguration,
            RosterProduct rosterProduct)
            : base(((ArmorIdentity)rosterProduct.Content).ToState(), armorConfiguration)
        {
            this.rosterProduct = rosterProduct;
            this.armorConfiguration = armorConfiguration;
        }

        public RosterProduct RosterProduct => rosterProduct;

        public Grade MaxGrade => CommonConfiguration.MaxGrade;

        public int MaxGradeMaxLevel => armorConfiguration.GetMaxLevel(MaxGrade);

        public ItemStats MaxStats => ItemStats.Sum(
            armorConfiguration.GetGradeStats(Name, MaxGrade),
            armorConfiguration.GetLevelStats(Name, MaxGradeMaxLevel));
    }
}