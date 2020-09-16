using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Game
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public Game(ServerPlayer player, ServerArmy army, Shop shop, IDateTimeProvider dateTimeProvider)
        {
            Player = player;
            Army = army;
            Shop = shop;

            this.dateTimeProvider = dateTimeProvider;
        }

        public ServerPlayer Player { get; }

        public ServerArmy Army { get; }

        public Shop Shop { get; set; }

        public bool CanLevelUpTrooperInstant(TrooperClass trooperClass)
        {
            if (!Army.CanLevelUpTrooper(trooperClass))
            {
                return false;
            }

            var price = Shop.GetTrooperInstantLevelUpPrice(trooperClass);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpTrooperRegular(TrooperClass trooperClass)
        {
            var trooper = Army.GetTrooper(trooperClass);
            if (trooper == null || !Army.CanStartItemProgress(trooper.Id))
            {
                return false;
            }

            var price = Shop.GetTrooperRegularLevelUpPrice(trooperClass);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanGardeUpTrooperInstant(TrooperClass trooperClass)
        {
            if (!Army.CanGradeUpTrooper(trooperClass))
            {
                return false;
            }

            var price = Shop.GetTrooperInstantGradeUpPrice(trooperClass);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpWeaponInstant(WeaponName weaponName)
        {
            if (!Army.CanLevelUpWeapon(weaponName))
            {
                return false;
            }

            var price = Shop.GetWeaponInstantLevelUpPrice(weaponName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpWeaponRegular(WeaponName weaponName)
        {
            var weapon = Army.GetWeapon(weaponName);
            if (weapon == null || !Army.CanStartItemProgress(weapon.Id))
            {
                return false;
            }

            var price = Shop.GetWeaponRegularLevelUpPrice(weaponName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanGardeUpWeaponInstant(WeaponName weaponName)
        {
            if (!Army.CanGradeUpWeapon(weaponName))
            {
                return false;
            }

            var price = Shop.GetWeaponInstantGradeUpPrice(weaponName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpHelmetInstant(HelmetName helmetName)
        {
            if (!Army.CanLevelUpHelmet(helmetName))
            {
                return false;
            }

            var price = Shop.GetHelmetInstantLevelUpPrice(helmetName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpHelmetRegular(HelmetName helmetName)
        {
            var helmet = Army.GetHelmet(helmetName);
            if (helmet == null || !Army.CanStartItemProgress(helmet.Id))
            {
                return false;
            }

            var price = Shop.GetHelmetRegularLevelUpPrice(helmetName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanGardeUpHelmetInstant(HelmetName helmetName)
        {
            if (!Army.CanGradeUpHelmet(helmetName))
            {
                return false;
            }

            var price = Shop.GetHelmetInstantGradeUpPrice(helmetName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpArmorInstant(ArmorName armorName)
        {
            if (!Army.CanLevelUpArmor(armorName))
            {
                return false;
            }

            var price = Shop.GetArmorInstantLevelUpPrice(armorName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanLevelUpArmorRegular(ArmorName armorName)
        {
            var armor = Army.GetArmor(armorName);
            if (armor == null || !Army.CanStartItemProgress(armor.Id))
            {
                return false;
            }

            var price = Shop.GetArmorRegularLevelUpPrice(armorName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanGardeUpArmorInstant(ArmorName armorName)
        {
            if (!Army.CanGradeUpArmor(armorName))
            {
                return false;
            }

            var price = Shop.GetArmorInstantGradeUpPrice(armorName);
            if (!Player.CanPayPrice(price))
            {
                return false;
            }

            return true;
        }

        public bool CanBuyRosterProduct(string productId)
        {
            if (!Shop.IsRosterProductExists(productId))
            {
                return false;
            }

            var product = Shop.GetRosterProduct(productId);

            return
                Player.CanPayPrice(product.Price) &&
                Player.Level >= product.PlayerLevelForUnlock &&
                !Army.HasContent(product.Content);
        }

        public bool CanBuyShopProduct(string productId)
        {
            if (!Shop.IsShopProductExists(productId))
            {
                return false;
            }

            var product = Shop.GetShopProduct(productId);

            return
                Player.CanPayPrice(product.Price) &&
                Player.Level >= product.MinPlayerLevel &&
                Player.Level <= product.MaxPlayerLevel &&
                !Army.HasAnyContent(product.Content);
        }

        public bool CanCompleteArmyItemProgress()
        {
            return
                Army.CanCompleteItemProgress() &&
                Player.CanPayPrice(Army.GetItemProgress().CompletePrice);
        }

        public ServerGameState GetState()
        {
            return new ServerGameState
            {
                Player = Player.GetState(),
                Army = Army.GetState(),
                UtcNow = dateTimeProvider.UtcNow,
            };
        }
    }
}
