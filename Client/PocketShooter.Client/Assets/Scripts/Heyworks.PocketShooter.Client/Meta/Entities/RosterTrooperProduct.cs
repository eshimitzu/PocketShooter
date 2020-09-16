using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Meta.Entities
{
    // TODO: a.dezhurko Bad inheritance.
    // TODO: v.shimkovich need only stats+config from base class, create separate one
    public class RosterTrooperProduct : TrooperBase
    {
        private readonly RosterProduct rosterProduct;
        private readonly ITrooperConfigurationBase trooperConfiguration;

        public RosterTrooperProduct(
            ITrooperConfigurationBase trooperConfiguration,
            RosterProduct rosterProduct)
            : base(((TrooperIdentity)rosterProduct.Content).ToState(), trooperConfiguration)
        {
            this.rosterProduct = rosterProduct;
            this.trooperConfiguration = trooperConfiguration;
        }

        public RosterProduct RosterProduct => rosterProduct;

        public Grade MaxGrade => CommonConfiguration.MaxGrade;

        public int MaxGradeMaxLevel => trooperConfiguration.GetMaxLevel(MaxGrade);

        public ItemStats MaxStats => ItemStats.Sum(
            trooperConfiguration.GetGradeStats(Class, MaxGrade),
            trooperConfiguration.GetLevelStats(Class, MaxGradeMaxLevel));
    }
}