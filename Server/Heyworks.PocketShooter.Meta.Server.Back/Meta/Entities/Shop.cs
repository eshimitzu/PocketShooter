using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Shop
    {
        private readonly ServerArmy army;
        private readonly IShopConfigurationBase shopConfiguration;

        public Shop(ServerArmy army, IShopConfigurationBase shopConfiguration)
        {
            this.army = army;
            this.shopConfiguration = shopConfiguration;
        }

        public Price GetTrooperInstantLevelUpPrice(TrooperClass trooperClass) =>
            army.GetTrooper(trooperClass).InstantLevelUpPrice;

        public Price GetTrooperRegularLevelUpPrice(TrooperClass trooperClass) =>
            army.GetTrooper(trooperClass).RegularLevelUpPrice;

        public Price GetTrooperInstantGradeUpPrice(TrooperClass trooperClass) =>
            army.GetTrooper(trooperClass).InstantGradeUpPrice;

        public Price GetWeaponInstantLevelUpPrice(WeaponName weaponName) =>
            army.GetWeapon(weaponName).InstantLevelUpPrice;

        public Price GetWeaponRegularLevelUpPrice(WeaponName weaponName) =>
            army.GetWeapon(weaponName).RegularLevelUpPrice;

        public Price GetWeaponInstantGradeUpPrice(WeaponName weaponName) =>
            army.GetWeapon(weaponName).InstantGradeUpPrice;

        public Price GetHelmetInstantLevelUpPrice(HelmetName helmetName) =>
            army.GetHelmet(helmetName).InstantLevelUpPrice;

        public Price GetHelmetRegularLevelUpPrice(HelmetName helmetName) =>
            army.GetHelmet(helmetName).RegularLevelUpPrice;

        public Price GetHelmetInstantGradeUpPrice(HelmetName helmetName) =>
            army.GetHelmet(helmetName).InstantGradeUpPrice;

        public Price GetArmorInstantLevelUpPrice(ArmorName armorName) =>
            army.GetArmor(armorName).InstantLevelUpPrice;

        public Price GetArmorRegularLevelUpPrice(ArmorName armorName) =>
            army.GetArmor(armorName).RegularLevelUpPrice;

        public Price GetArmorInstantGradeUpPrice(ArmorName armorName) =>
            army.GetArmor(armorName).InstantGradeUpPrice;

        public InAppPurchase GetPurchase(string purchaseId) => shopConfiguration.GetPurchase(purchaseId);

        public IEnumerable<IContentIdentity> GetPurchaseContent(string purchaseId) => shopConfiguration.GetPurchaseContent(purchaseId);

        public bool IsRosterProductExists(string productId) => shopConfiguration.IsRosterProductExists(productId);

        public RosterProductBase GetRosterProduct(string productId) =>
            IsRosterProductExists(productId)
            ? new RosterProductBase(productId, shopConfiguration)
            : throw new InvalidOperationException($"The roster product with id {productId} does not exists.");

        public bool IsShopProductExists(string productId) => shopConfiguration.IsShopProductExists(productId);

        public ShopProductBase GetShopProduct(string productId) =>
            IsShopProductExists(productId)
            ? new ShopProductBase(productId, shopConfiguration)
            : throw new InvalidOperationException($"The shop product with id {productId} does not exists.");
    }
}
