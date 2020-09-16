using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.UI.Lobby
{
    public static class MetaGameExtensions
    {
        public static List<ILobbyRosterItem> GetTrooperItems(this MetaGame game) =>
            game.Army.Troopers
                .Select(trooper => new TrooperRosterItem(trooper))
                .OrderBy(trooper => trooper.Power)
                .Concat<ILobbyRosterItem>(
                    game.Roster.GetAvailableTroopers()
                        .Select(trooper => new TrooperProductRosterItem(trooper))
                        .OrderBy(trooper => trooper.Product.IsLocked ? trooper.Product.PlayerLevelForUnlock : 0)
                        .ThenBy(trooper => trooper.Power))
                .ToList();

        public static List<ILobbyRosterItem> GetWeaponItems(this MetaGame game) =>
            game.Army.Weapons
                .Select(weapon => new WeaponRosterItem(weapon))
                .OrderBy(weapon => weapon.Power)
                .Concat<ILobbyRosterItem>(
                    game.Roster.GetAvailableWeapons()
                        .Select(weapon => new WeaponProductRosterItem(weapon))
                        .OrderBy(weapon => weapon.Product.IsLocked ? weapon.Product.PlayerLevelForUnlock : 0)
                        .ThenBy(weapon => weapon.Power))
                .ToList();

        public static List<ILobbyRosterItem> GetHelmetItems(this MetaGame game) =>
            game.Army.Helmets
                .Select(helmet => new HelmetRosterItem(helmet))
                .OrderBy(helmet => helmet.Power)
                .Concat<ILobbyRosterItem>(
                    game.Roster.GetAvailableHelmets()
                        .Select(helmet => new HelmetProductRosterItem(helmet))
                        .OrderBy(helmet => helmet.Product.IsLocked ? helmet.Product.PlayerLevelForUnlock : 0)
                        .ThenBy(helmet => helmet.Power))
                .ToList();

        public static List<ILobbyRosterItem> GetArmorItems(this MetaGame game) =>
            game.Army.Armors
                .Select(armor => new ArmorRosterItem(armor))
                .OrderBy(armor => armor.Power)
                .Concat<ILobbyRosterItem>(
                    game.Roster.GetAvailableArmors()
                        .Select(armor => new ArmorProductRosterItem(armor))
                        .OrderBy(armor => armor.Product.IsLocked ? armor.Product.PlayerLevelForUnlock : 0)
                        .ThenBy(armor => armor.Power))
                .ToList();
    }
}