using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Meta.Entities
{
    // TODO: a.dezhurko Bad inheritance.
    // TODO: v.shimkovich need only stats+config from base class, create separate one
    public class RosterHelmetProduct : HelmetBase
    {
        private readonly RosterProduct rosterProduct;
        private readonly IHelmetConfigurationBase helmetConfiguration;

        public RosterHelmetProduct(
            IHelmetConfigurationBase helmetConfiguration,
            RosterProduct rosterProduct)
            : base(((HelmetIdentity)rosterProduct.Content).ToState(), helmetConfiguration)
        {
            this.rosterProduct = rosterProduct;
            this.helmetConfiguration = helmetConfiguration;
        }

        public RosterProduct RosterProduct => rosterProduct;

        public Grade MaxGrade => CommonConfiguration.MaxGrade;

        public int MaxGradeMaxLevel => helmetConfiguration.GetMaxLevel(MaxGrade);

        public ItemStats MaxStats => ItemStats.Sum(
            helmetConfiguration.GetGradeStats(Name, MaxGrade),
            helmetConfiguration.GetLevelStats(Name, MaxGradeMaxLevel));
    }
}