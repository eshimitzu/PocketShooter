using System;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.EndBattle;
using Heyworks.PocketShooter.UI.Popups;

namespace Heyworks.PocketShooter.UI.Localization
{
    public static class LocKeys
    {
        public const string Health = "EntityInfo/Info_Health";
        public const string Movement = "EntityInfo/Info_Speed";
        public const string Armor = "EntityInfo/Info_Armor";
        public const string Capacity = "EntityInfo/Info_Ammo";
        public const string Distance = "EntityInfo/Info_Range";
        public const string Attack = "EntityInfo/Info_Damage";
        public const string Reload = "EntityInfo/Info_Recharge";

        public const string Power = "EntityInfo/Info_Power";
        public const string MaxPower = "EntityInfo/Info_Max_Power";
        public const string PowerShortest = "EntityInfo/Power_Shortest";
        public const string Level = "EntityInfo/Info_Lvl";
        public const string MaxLevel = "EntityInfo/Info_Max_Lvl";
        public const string LevelShort = "EntityInfo/Roaster_Lvl";

        public const string SecondsShort = "Battle/Battle_Loader_Timer";

        public const string SettingsOn = "PopUPs/Sound_Turn_On";
        public const string SettingsOff = "PopUPs/Sound_Turn_Off";
        public const string AccountLevelUp = "PopUPs/Account_Level_Up";
        public const string SocialNetworkGoogle = "PopUPs/Social_Google";
        public const string LogIn = "PopUPs/Log_In";
        public const string LogOut = "PopUPs/Log_Out";
        public const string BattleDisconnectTitle = "PopUPs/Battle_Disconnect_Title";
        public const string BattleDisconnectDescription = "PopUPs/Battle_Disconnect_Description";
        public const string BattleDisconnectButton = "PopUPs/Battle_Disconnect_Button";
        public const string MetaDisconnectTitle = "PopUPs/Meta_Disconnect_Title";
        public const string MetaDisconnectDescription = "PopUPs/Meta_Disconnect_Description";
        public const string MetaDisconnectButton = "PopUPs/Meta_Disconnect_Button";

        public const string PointA = "Battle/Point_A";

        public const string PointB = "Battle/Point_B";

        public const string PointC = "Battle/Point_C";

        public const string LevelUp = "PopUPs/Level_Up";
        public const string Evolution = "PopUPs/Evolution";
        public const string ClaimReward = "PopUPs/Claim_Reward";

        public const string KillSeries = "Battle/Kill_Series_";
        public const string MatchEnded = "Battle/Match_Ended";

        public static class ItemInfo
        {
            public const string Buy = "Shop/Buy";
            public const string Upgrade = "EntityInfo/Info_Upgrade";
            public const string Train = "EntityInfo/Info_LvlUP";
            public const string GradeUp = "EntityInfo/Info_Evolve";
            public const string Equip = "EntityInfo/Info_Equip";
            public const string Equipped = "EntityInfo/Info_Equiped";
            public const string InstantDuration = "EntityInfo/Info_Instant";
            public const string LevelUpInProgress = "EntityInfo/Info_Training";
            public const string InstantComplete = "EntityInfo/Info_Complete";
        }

        public static class Offers
        {
            public const string OfferNameChuckNorris = "Shop/Character_Offer_Name_Ranger";
            public const string OfferNameNeo = "Shop/Character_Offer_Name_Neo";
            public const string OfferNameStatham = "Shop/Character_Offer_Name_Bacon";
            public const string OfferNameRock = "Shop/Character_Offer_Name_Bull";
            public const string OfferNameArmor2 = "Shop/Offer_Name_Armor_2";
            public const string OfferNameArmor3 = "Shop/Offer_Name_Armor_3";
            public const string OfferNameArmor4 = "Shop/Offer_Name_Armor_4";
            public const string OfferNameArmor5 = "Shop/Offer_Name_Armor_5";

            public const string OfferDescriptionChuckNorris = "Shop/Character_Offer_Desc_Ranger";
            public const string OfferDescriptionNeo = "Shop/Character_Offer_Desc_Neo";
            public const string OfferDescriptionStatham = "Shop/Character_Offer_Desc_Bacon";
            public const string OfferDescriptionRock = "Shop/Character_Offer_Desc_Bull";
            public const string OfferDescriptionArmor2 = "Shop/Offer_Description_Armor_2";
            public const string OfferDescriptionArmor3 = "Shop/Offer_Description_Armor_3";
            public const string OfferDescriptionArmor4 = "Shop/Offer_Description_Armor_4";
            public const string OfferDescriptionArmor5 = "Shop/Offer_Description_Armor_5";

            public static string GetOfferNameKey(OfferType offerType)
            {
                switch (offerType)
                {
                    case OfferType.Norris: return OfferNameChuckNorris;
                    case OfferType.Neo: return OfferNameNeo;
                    case OfferType.Statham: return OfferNameStatham;
                    case OfferType.Rock: return OfferNameRock;
                    case OfferType.Armor2: return OfferNameArmor2;
                    case OfferType.Armor3: return OfferNameArmor3;
                    case OfferType.Armor4: return OfferNameArmor4;
                    case OfferType.Armor5: return OfferNameArmor5;
                    default: throw new ArgumentException(nameof(offerType));
                }
            }

            public static string GetOfferDescriptionKey(OfferType offerType)
            {
                switch (offerType)
                {
                    case OfferType.Norris: return OfferDescriptionChuckNorris;
                    case OfferType.Neo: return OfferDescriptionNeo;
                    case OfferType.Statham: return OfferDescriptionStatham;
                    case OfferType.Rock: return OfferDescriptionRock;
                    case OfferType.Armor2: return OfferDescriptionArmor2;
                    case OfferType.Armor3: return OfferDescriptionArmor3;
                    case OfferType.Armor4: return OfferDescriptionArmor4;
                    case OfferType.Armor5: return OfferDescriptionArmor5;
                    default: throw new ArgumentException(nameof(offerType));
                }
            }
        }

        public static class Languages
        {
            public static class SupprtedLanguages
            {
                public const string English = "English";
                public const string Russian = "Russian";
            }

            public const string English = "PopUPs/Language_English";
            public const string Russian = "PopUPs/Language_Russian";

            public static string GetSupportedLanguageByKey(string key)
            {
                switch (key)
                {
                    case English: return SupprtedLanguages.English;
                    case Russian: return SupprtedLanguages.Russian;
                    default: return SupprtedLanguages.English;
                }
            }
        }

        public static class TrooperNames
        {
            public const string Sniper = "SoldierClasses/Character_Name_Sniper";
            public const string Rambo = "SoldierClasses/Character_Name_Soldier";
            public const string Scout = "SoldierClasses/Character_Name_Scout";
            public const string Spy = "SoldierClasses/Character_Name_Spy";
            public const string Neo = "SoldierClasses/Character_Name_Breathtaking";
            public const string Norris = "SoldierClasses/Character_Name_Ranger";
            public const string Rock = "SoldierClasses/Character_Name_Bull";
            public const string Statham = "SoldierClasses/Character_Name_Bacon";
            public const string Stallone = "SoldierClasses/Character_Name_Stallone";
            public const string Terminator = "SoldierClasses/Character_Name_Terminator";
            public const string Junkrat = "SoldierClasses/Character_Name_Junkrat";
            public const string Spidy = "SoldierClasses/Character_Name_Spidy";
            public const string Irokez = "SoldierClasses/Character_Name_Irokez";
            public const string Stryker = "SoldierClasses/Character_Name_Stryker";
            public const string WillSmith = "SoldierClasses/Character_Name_WillSmith";
            public const string McClane = "SoldierClasses/Character_Name_McClane";
            public const string MadMax = "SoldierClasses/Character_Name_MadMax";
            public const string PUBG = "SoldierClasses/Character_Name_PUBG";
        }

        public static class WeaponNames
        {
            public const string M16A4 = "Items/Weapon_M16";
            public const string Remington = "Items/Weapon_Remington";
            public const string SVD = "Items/Weapon_SVD";
            public const string Knife = "Items/Weapon_Knife";
            public const string SawedOff = "Items/Weapon_Sawedoff";
            public const string Katana = "Items/Weapon_Katana";
            public const string Barret = "Items/Weapon_Barret";
            public const string Minigun = "Items/Weapon_Minigun";
        }

        public static class HelmetNames
        {
            public const string Helmet1 = "Items/Helmet_Helmet1";
            public const string Helmet2 = "Items/Helmet_Helmet2";
            public const string Helmet3 = "Items/Helmet_Helmet3";
            public const string Helmet4 = "Items/Helmet_Helmet4";
            public const string Helmet5 = "Items/Helmet_Helmet5";
        }

        public static class ArmorNames
        {
            public const string Armor1 = "Items/Armor_Armor1";
            public const string Armor2 = "Items/Armor_Armor2";
            public const string Armor3 = "Items/Armor_Armor3";
            public const string Armor4 = "Items/Armor_Armor4";
            public const string Armor5 = "Items/Armor_Armor5";
        }

        public static string GetResultKey(WinLoosView.Result result)
        {
            switch (result)
            {
                case WinLoosView.Result.Win: return "AfterBattle/AfterBattle_Win";
                case WinLoosView.Result.Draw: return "AfterBattle/AfterBattle_Draw";
                case WinLoosView.Result.Lose: return "AfterBattle/AfterBattle_Lose";
                default: throw new ArgumentException(nameof(result));
            }
        }

        public static class ShopCategoryNames
        {
            public const string Offers = "Shop/Tab_Name_Offers";
            public const string Hard = "Shop/Tab_Name_Gold";
            public const string Soft = "Shop/Tab_Name_Cash";
            public const string Consumables = "Shop/Tab_Name_Consumables";
        }

        public static string GetContentNameKey(IContentIdentity content)
        {
            switch (content)
            {
                case TrooperIdentity t:
                    return GetTooperNameKey(t.Class);
                case WeaponIdentity w:
                    return GetWeaponNameKey(w.Name);
                case HelmetIdentity h:
                    return GetHelmetNameKey(h.Name);
                case ArmorIdentity a:
                    return GetArmorNameKey(a.Name);
                default:
                    return string.Empty;
            }
        }

        public static string GetShopCategoryKey(ShopCategory categoryType)
        {
            switch (categoryType)
            {
                case ShopCategory.Offers: return ShopCategoryNames.Offers;
                case ShopCategory.Hard: return ShopCategoryNames.Hard;
                case ShopCategory.Soft: return ShopCategoryNames.Soft;
                case ShopCategory.Consumables: return ShopCategoryNames.Consumables;
                default: throw new ArgumentException(nameof(categoryType));
            }
        }

        public static string GetTooperNameKey(TrooperClass trooperClass)
        {
            switch (trooperClass)
            {
                case TrooperClass.Rambo: return TrooperNames.Rambo;
                case TrooperClass.Scout: return TrooperNames.Scout;
                case TrooperClass.Sniper: return TrooperNames.Sniper;
                case TrooperClass.Spy: return TrooperNames.Spy;
                case TrooperClass.Norris: return TrooperNames.Norris;
                case TrooperClass.Neo: return TrooperNames.Neo;
                case TrooperClass.Statham: return TrooperNames.Statham;
                case TrooperClass.Rock: return TrooperNames.Rock;
                case TrooperClass.Stallone: return TrooperNames.Stallone;
                case TrooperClass.Terminator: return TrooperNames.Terminator;
                case TrooperClass.Junkrat: return TrooperNames.Junkrat;
                case TrooperClass.Spidy: return TrooperNames.Spidy;
                case TrooperClass.Irokez: return TrooperNames.Irokez;
                case TrooperClass.Stryker: return TrooperNames.Stryker;
                case TrooperClass.WillSmith: return TrooperNames.WillSmith;
                case TrooperClass.McClane: return TrooperNames.McClane;
                case TrooperClass.MadMax: return TrooperNames.MadMax;
                case TrooperClass.PUBG: return TrooperNames.PUBG;
                default: throw new ArgumentException(nameof(trooperClass));
            }
        }

        public static string GetWeaponNameKey(WeaponName weaponName)
        {
            switch (weaponName)
            {
                case WeaponName.M16A4: return WeaponNames.M16A4;
                case WeaponName.SawedOff: return WeaponNames.SawedOff;
                case WeaponName.SVD: return WeaponNames.SVD;
                case WeaponName.Knife: return WeaponNames.Knife;
                case WeaponName.Remington: return WeaponNames.Remington;
                case WeaponName.Katana: return WeaponNames.Katana;
                case WeaponName.Barret: return WeaponNames.Barret;
                case WeaponName.Minigun: return WeaponNames.Minigun;
                default: throw new ArgumentException(nameof(weaponName));
            }
        }

        public static string GetHelmetNameKey(HelmetName helmetName)
        {
            switch (helmetName)
            {
                case HelmetName.Helmet1: return HelmetNames.Helmet1;
                case HelmetName.Helmet2: return HelmetNames.Helmet2;
                case HelmetName.Helmet3: return HelmetNames.Helmet3;
                case HelmetName.Helmet4: return HelmetNames.Helmet4;
                case HelmetName.Helmet5: return HelmetNames.Helmet5;
                default: throw new ArgumentException(nameof(helmetName));
            }
        }

        public static string GetArmorNameKey(ArmorName armorName)
        {
            switch (armorName)
            {
                case ArmorName.Armor1: return ArmorNames.Armor1;
                case ArmorName.Armor2: return ArmorNames.Armor2;
                case ArmorName.Armor3: return ArmorNames.Armor3;
                case ArmorName.Armor4: return ArmorNames.Armor4;
                case ArmorName.Armor5: return ArmorNames.Armor5;
                default: throw new ArgumentException(nameof(armorName));
            }
        }

        public static class Skill
        {
            public const string StunningHit = "Abilities/Ability_Name_PainShock";
            public const string RocketJump = "Abilities/Ability_Name_Jump";
            public const string RootingHit = "Abilities/Ability_Name_Trap";
            public const string Lifesteal = "Abilities/Ability_Name_VampirismAura";
            public const string Invisibility = "Abilities/Ability_Name_Camouflage";
            public const string Immortality = "Abilities/Ability_Name_DefenseMatrix";
            public const string ShockWave = "Abilities/Ability_Name_Stunning";
            public const string ShockWaveJump = "Abilities/Ability_Name_ShockWave";
            public const string ExtraDamage = "Abilities/Ability_Name_OverDamage";
            public const string LifeDrain = "Abilities/Ability_Name_Lifesteal";
            public const string Sprint = "Abilities/Ability_Name_Sprint";
            public const string InstantReload = "Abilities/Ability_Name_SpareClip";
            public const string Rage = "Abilities/Ability_Name_Rage";
            public const string RegenerationOnKill = "Abilities/Ability_Name_Harvest";
            public const string StealthDash = "Abilities/Ability_Name_Dash";
            public const string Lucky = "Abilities/Ability_Name_Endurance";
            public const string DoubleStealthDash = "Abilities/Ability_Name_DoubleDash";
            public const string Dive = "Abilities/Ability_Name_LaunchSystem";
            public const string StealthSprint = "Abilities/Ability_Name_SilentRun";
            public const string Heal = "Abilities/Ability_Name_AmbulanceBox";
            public const string ControlFreak = "Abilities/Ability_Name_Anatomy";
        }

        public static class SkillDescription
        {
            public const string StunningHit = "Abilities/Ability_Description_PainShock";
            public const string RocketJump = "Abilities/Ability_Description_Jump";
            public const string RootingHit = "Abilities/Ability_Description_Trap";
            public const string Lifesteal = "Abilities/Ability_Description_VampirismAura";
            public const string Invisibility = "Abilities/Ability_Description_Camouflage";
            public const string Immortality = "Abilities/Ability_Description_DefenseMatrix";
            public const string ShockWave = "Abilities/Ability_Description_Stunning";
            public const string ShockWaveJump = "Abilities/Ability_Description_ShockWave";
            public const string ExtraDamage = "Abilities/Ability_Description_OverDamage";
            public const string LifeDrain = "Abilities/Ability_Description_Lifesteal";
            public const string Sprint = "Abilities/Ability_Description_Sprint";
            public const string InstantReload = "Abilities/Ability_Description_SpareClip";
            public const string Rage = "Abilities/Ability_Description_Dash";
            public const string RegenerationOnKill = "Abilities/Ability_Description_Harvest";
            public const string StealthDash = "Abilities/Ability_Description_Dash";
            public const string Lucky = "Abilities/Ability_Description_Endurance";
            public const string DoubleStealthDash = "Abilities/Ability_Description_DoubleDash";
            public const string Dive = "Abilities/Ability_Description_LaunchSystem";
            public const string StealthSprint = "Abilities/Ability_Description_SilentRun";
            public const string Heal = "Abilities/Ability_Description_AmbulanceBox";
            public const string ControlFreak = "Abilities/Ability_Description_Anatomy";
        }

        public static string GetLocKeyForSkillName(SkillName skillName)
        {
            switch (skillName)
            {
                case SkillName.StunningHit: return Skill.StunningHit;
                case SkillName.RocketJump: return Skill.RocketJump;
                case SkillName.RootingHit: return Skill.RootingHit;
                case SkillName.Lifesteal: return Skill.Lifesteal;
                case SkillName.Invisibility: return Skill.Invisibility;
                case SkillName.Immortality: return Skill.Immortality;
                case SkillName.ShockWave: return Skill.ShockWave;
                case SkillName.ShockWaveJump: return Skill.ShockWaveJump;
                case SkillName.ExtraDamage: return Skill.ExtraDamage;
                case SkillName.Sprint: return Skill.Sprint;
                case SkillName.InstantReload: return Skill.InstantReload;
                case SkillName.Rage: return Skill.Rage;
                case SkillName.RegenerationOnKill: return Skill.RegenerationOnKill;
                case SkillName.StealthDash: return Skill.StealthDash;
                case SkillName.Lucky: return Skill.Lucky;
                case SkillName.DoubleStealthDash: return Skill.DoubleStealthDash;
                case SkillName.Dive: return Skill.Dive;
                case SkillName.StealthSprint: return Skill.StealthSprint;
                case SkillName.Heal: return Skill.Heal;
                case SkillName.ControlFreak: return Skill.ControlFreak;
                case SkillName.LifeDrain: return Skill.LifeDrain;
                default: throw new ArgumentException($"Lock key is not provided for skill {skillName}.");
            }
        }

        public static string GetLocKeyForDescriptionSkillName(SkillName skillName)
        {
            switch (skillName)
            {
                case SkillName.StunningHit: return SkillDescription.StunningHit;
                case SkillName.RocketJump: return SkillDescription.RocketJump;
                case SkillName.RootingHit: return SkillDescription.RootingHit;
                case SkillName.Lifesteal: return SkillDescription.Lifesteal;
                case SkillName.Invisibility: return SkillDescription.Invisibility;
                case SkillName.Immortality: return SkillDescription.Immortality;
                case SkillName.ShockWave: return SkillDescription.ShockWave;
                case SkillName.ShockWaveJump: return SkillDescription.ShockWaveJump;
                case SkillName.ExtraDamage: return SkillDescription.ExtraDamage;
                case SkillName.Sprint: return SkillDescription.Sprint;
                case SkillName.InstantReload: return SkillDescription.InstantReload;
                case SkillName.Rage: return SkillDescription.Rage;
                case SkillName.RegenerationOnKill: return SkillDescription.RegenerationOnKill;
                case SkillName.StealthDash: return SkillDescription.StealthDash;
                case SkillName.Lucky: return SkillDescription.Lucky;
                case SkillName.DoubleStealthDash: return SkillDescription.DoubleStealthDash;
                case SkillName.Dive: return SkillDescription.Dive;
                case SkillName.StealthSprint: return SkillDescription.StealthSprint;
                case SkillName.Heal: return SkillDescription.Heal;
                case SkillName.ControlFreak: return SkillDescription.ControlFreak;
                case SkillName.LifeDrain: return SkillDescription.LifeDrain;
                default: throw new ArgumentException($"Lock key is not provided for skill description {skillName}.");
            }
        }

        public static string GetShopItemKey(string id)
        {
            return $"ShopItems/{id}";
        }

        public static class SocialNetwork
        {
            public const string GameCenter = "_GC";
            public const string GooglePlay = "_GP";
        }

        public static string GetSocialNetworkName(SocialNetworkName socialNetworkName)
        {
            switch (socialNetworkName)
            {
                case SocialNetworkName.GameCenter: return SocialNetwork.GameCenter;
                case SocialNetworkName.GooglePlay: return SocialNetwork.GooglePlay;
                default: throw new ArgumentOutOfRangeException(nameof(socialNetworkName), socialNetworkName, null);
            }
        }

        /// <summary>
        /// Class-container for constants representing localization keys of texts from "Time" localization group.
        /// </summary>
        public static class Time
        {
            public const string MONTHS_TEMPLATE = "Time/{0} months";
            public const string WEEKS_TEMPLATE = "Time/{0} weeks";

            public const string DAYS_TEMPLATE = "Time/{0} days";
            public const string HOURS_TEMPLATE = "Time/{0} hours";
            public const string MINUTES_TEMPLATE = "Time/{0} minutes";
            public const string SECONDS_TEMPLATE = "Time/{0} seconds";

            public const string DAYS_TEMPLATE_SHORT = "Time/{0}d";
            public const string HOURS_TEMPLATE_SHORT = "Time/{0}h";
            public const string MINUTES_TEMPLATE_SHORT = "Time/{0}m";
            public const string SECONDS_TEMPLATE_SHORT = "Time/{0}s";

            public const string DAYS_HOURS_TEMPLATE_SHORT = "Time/{0}d {1}h";
            public const string HOURS_MINUTES_TEMPLATE_SHORT = "Time/{0}h {1}m";
            public const string MINUTES_SECONDS_TEMPLATE_SHORT = "Time/{0}m {1}s";

            public const string ONE_MONTH = "Time/1 month";
            public const string ONE_WEEK = "Time/1 week";
            public const string ONE_DAY = "Time/1 day";
            public const string ONE_HOUR = "Time/1 hour";
            public const string ONE_MINUTE = "Time/1 minute";
            public const string ONE_SECOND = "Time/1 second";
            public const string AGO_TEMPLATE = "Time/{0} ago";

            public const string AGO_NOW_TEMPLATE = "Time/now";
            public const string AGO_MINUTE_TEMPLATE = "Time/{0}m ago";
            public const string AGO_HOUR_TEMPLATE = "Time/{0}h ago";
            public const string AGO_DAY_TEMPLATE = "Time/{0}d ago";
            public const string AGO_WEEK_TEMPLATE = "Time/{0}w ago";
            public const string AGO_MONTH_TEMPLATE = "Time/{0}m ago";
            public const string AGO_YEAR_TEMPLATE = "Time/{0} year(s) ago";
        }

        public static class Social
        {
            public const string Title = "Social/ErrorPopup_Title";
            public const string RogerButton = "Social/ErrorPopup_RogerButton";
            public const string SocialNetworkError = "Social/ErrorPopup_SocialNetworkError_{0}";
            public const string UserAlreadyConnected = "Social/ErrorPopup_UserAlreadyConnected";
            public const string UnknownAuthError = "Social/ErrorPopup_UnknownAuthError";
            public const string SocialAccountSwitchOrSkip = "Social/ErrorPopup_SocialAccountSwitchOrSkip";

            public static string GetSocialNetworkError(SocialNetworkName name) =>
                string.Format(SocialNetworkError, name);

            // TODO: v.shimkovich temp solution. this is not key but translated string which won't be auto localized.
            public static string GetSocialAccountSwitchOrSkipLocalized(string currentArmy, string accountName, string accountArmy) =>
                string.Format(SocialAccountSwitchOrSkip.Localized(), currentArmy, accountName, accountArmy);
        }
    }
}