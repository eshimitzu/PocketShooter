using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Purchasing.Core;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Roster
    {
        private readonly Shop shop;
        private readonly Army army;
        private readonly ITrooperConfigurationBase trooperConfiguration;
        private readonly IWeaponConfigurationBase weaponConfiguration;
        private readonly IHelmetConfigurationBase helmetConfiguration;
        private readonly IArmorConfigurationBase armorConfiguration;

        public Roster(
            Shop shop,
            Army army,
            ITrooperConfigurationBase trooperConfiguration,
            IWeaponConfigurationBase weaponConfiguration,
            IHelmetConfigurationBase helmetConfiguration,
            IArmorConfigurationBase armorConfiguration)
        {
            this.shop = shop;
            this.army = army;
            this.trooperConfiguration = trooperConfiguration;
            this.weaponConfiguration = weaponConfiguration;
            this.helmetConfiguration = helmetConfiguration;
            this.armorConfiguration = armorConfiguration;
        }

        public IEnumerable<RosterTrooperProduct> GetAvailableTroopers()
        {
            var troopers = new List<RosterTrooperProduct>();
            var trooperRosterProducts = GetRosterProducts<TrooperIdentity>();

            foreach (var product in trooperRosterProducts)
            {
                troopers.Add(new RosterTrooperProduct(trooperConfiguration, product));
            }

            return troopers;
        }

        public IEnumerable<RosterWeaponProduct> GetAvailableWeapons()
        {
            var weapons = new List<RosterWeaponProduct>();
            var weaponsRosterProducts = GetRosterProducts<WeaponIdentity>();

            foreach (var product in weaponsRosterProducts)
            {
                weapons.Add(new RosterWeaponProduct(weaponConfiguration, product));
            }

            return weapons;
        }

        public IEnumerable<RosterHelmetProduct> GetAvailableHelmets()
        {
            var helmets = new List<RosterHelmetProduct>();
            var helmetRosterProducts = GetRosterProducts<HelmetIdentity>();
            foreach (var product in helmetRosterProducts)
            {
                helmets.Add(new RosterHelmetProduct(helmetConfiguration, product));
            }

            return helmets;
        }

        public IEnumerable<RosterArmorProduct> GetAvailableArmors()
        {
            var armors = new List<RosterArmorProduct>();
            var armorRosterProducts = GetRosterProducts<ArmorIdentity>();
            foreach (var product in armorRosterProducts)
            {
                armors.Add(new RosterArmorProduct(armorConfiguration, product));
            }

            return armors;
        }

        private IEnumerable<RosterProduct> GetRosterProducts<T>()
            where T : IContentIdentity
        {
            return shop.GetShopProducts(
                p => p is RosterProduct rosterProduct &&
                     !rosterProduct.IsPurchased &&
                     rosterProduct.Content is T).Select(p => (RosterProduct)p);
        }
    }
}