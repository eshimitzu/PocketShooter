using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Meta.Entities
{
    // TODO: a.dezhurko Bad inheritance.
    // TODO: v.shimkovich need only stats+config from base class, create separate one
    public class RosterWeaponProduct : WeaponBase
    {
        private readonly RosterProduct rosterProduct;
        private readonly IWeaponConfigurationBase weaponConfiguration;

        public RosterWeaponProduct(
            IWeaponConfigurationBase weaponConfiguration,
            RosterProduct rosterProduct)
            : base(((WeaponIdentity)rosterProduct.Content).ToState(), weaponConfiguration)
        {
            this.rosterProduct = rosterProduct;
            this.weaponConfiguration = weaponConfiguration;
        }

        public RosterProduct RosterProduct => rosterProduct;

        public Grade MaxGrade => CommonConfiguration.MaxGrade;

        public int MaxGradeMaxLevel => weaponConfiguration.GetMaxLevel(MaxGrade);

        public ItemStats MaxStats => ItemStats.Sum(
            weaponConfiguration.GetGradeStats(Name, MaxGrade),
            weaponConfiguration.GetLevelStats(Name, MaxGradeMaxLevel));
    }
}