using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using NJsonSchema.Annotations;
using Heyworks.PocketShooter.Meta.Entities;
using System.Linq;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class GameConfig
    {
        private IDictionary<(TrooperClass Class, int Level), TrooperLevelConfig> trooperLevelConfigLookup;
        private IDictionary<(TrooperClass Class, Grade Grade), TrooperGradeConfig> trooperGradeConfigLookup;

        private IDictionary<(WeaponName Name, int Level), WeaponLevelConfig> weaponLevelConfigLookup;
        private IDictionary<(WeaponName Name, Grade Grade), WeaponGradeConfig> weaponGradeConfigLookup;

        private IDictionary<(HelmetName Name, int Level), HelmetLevelConfig> helmetLevelConfigLookup;
        private IDictionary<(HelmetName Name, Grade Grade), HelmetGradeConfig> helmetGradeConfigLookup;

        private IDictionary<(ArmorName Name, int Level), ArmorLevelConfig> armorLevelConfigLookup;
        private IDictionary<(ArmorName Name, Grade Grade), ArmorGradeConfig> armorGradeConfigLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfig"/> class.
        /// The config Id.
        /// </summary>
        public static readonly string ConfigId = nameof(GameConfig);

        public string Id { get; private set; } = ConfigId;

        [System.ComponentModel.Description("Configuration version. Must be update on each modification. First 2 numbers for game releases, 3rd for game fixes and incompatible changes, 4th for A/B testing and tuning")]
        [NotNull] // ask to support C# not nullabe attribute compiled into IL from C# 8.0 feature
        [JsonSchemaTypeAttribute(typeof(string))] // TODO: ask to support version validation out of box, so next to lines could be omitted
        [RegularExpression(@"^(\d+)(\.\d+)?(\.\d+)?(\.\d+)?$")]
        public Version Version { get; set; } = new Version("0.2.4");

        /// <summary>
        /// Gets or sets a configuration for basic prices.
        /// </summary>
        [Required]
        public BasicPricesConfig BasicPricesConfig { get; set; } = new BasicPricesConfig
        {
            OneMinuteInGold = 1.0,
            FreeTimeLimit = TimeSpan.FromMinutes(5),
        };

        /// <summary>
        /// Gets or sets the configuration of player levels.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        [System.ComponentModel.Description("Controls how much experience needed to jump from previous level to next")]
        public IList<PlayerLevelConfig> PlayerLevelsConfig { get; set; } = new List<PlayerLevelConfig>()
        {
            new PlayerLevelConfig
            {
                Level = 1,
                ExperienceInLevel = 200,
                CashBattleRewardBase = 100,
                GoldBattleRewardBase = 5,
                ExpBattleRewardBase = 10,
                Reward =
                {
                    new WeaponIdentity(WeaponName.M16A4, Grade.Common, 1),
                    new TrooperIdentity(TrooperClass.Rambo, Grade.Common, 1),
                    new ResourceIdentity(1000, 100)
                },
            },
            new PlayerLevelConfig
            {
                Level = 2,
                ExperienceInLevel = 400,
                CashBattleRewardBase = 100,
                GoldBattleRewardBase = 5,
                ExpBattleRewardBase = 10,
                Reward = { new ResourceIdentity(1000, 100) },
            },
            new PlayerLevelConfig
            {
                Level = 3,
                ExperienceInLevel = 600,
                CashBattleRewardBase = 100,
                GoldBattleRewardBase = 5,
                ExpBattleRewardBase = 10,
                Reward = { new ResourceIdentity(1000, 100) },
            },
            new PlayerLevelConfig
            {
                Level = 4,
                ExperienceInLevel = 800,
                CashBattleRewardBase = 100,
                GoldBattleRewardBase = 5,
                ExpBattleRewardBase = 10,
                Reward = { new ResourceIdentity(1000, 100) },
            },
            new PlayerLevelConfig
            {
                Level = 5,
                ExperienceInLevel = 1000,
                CashBattleRewardBase = 100,
                GoldBattleRewardBase = 5,
                ExpBattleRewardBase = 10,
                Reward = { new ResourceIdentity(1000, 100) },
            }
        };

        /// <summary>
        /// Gets the configuration of available in-app purchases.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<InAppPurchase> InAppPurchasesConfig { get; set; } = new List<InAppPurchase>()
        {
            new InAppPurchase
            {
                Id = "com.pocket.shooter.somecoins",
                PriceUSD = 0.99,
                Type = InAppPurchaseType.Consumable,
            },
            new InAppPurchase
            {
                Id = "com.pocket.shooter.rock",
                PriceUSD = 19.99,
                Type = InAppPurchaseType.Consumable,
            }
        };

        /// <summary>
        /// Gets the configuration of available content packs.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<ContentPackConfig> ContentPacksConfig { get; set; } = new List<ContentPackConfig>()
        {
            new ContentPackConfig
            {
                Id = "Gold_1000",
                Content = { new ResourceIdentity(0, 1000) }
            },
            new ContentPackConfig
            {
                Id = "Norris_Uncommon_1",
                Content = { new TrooperIdentity(TrooperClass.Norris, Grade.Uncommon, 1) }
            },
            new ContentPackConfig
            {
                Id = "Rock_Legendary_1",
                Content = { new TrooperIdentity(TrooperClass.Rock, Grade.Legendary, 1) }
            },
            new ContentPackConfig
            {
                Id = "Minigun_Uncommon_1",
                Content = { new WeaponIdentity(WeaponName.Minigun, Grade.Uncommon, 1) }
            },
            new ContentPackConfig
            {
                Id = "Helmet1_Common_1",
                Content = { new HelmetIdentity(HelmetName.Helmet1, Grade.Common, 1) }
            },
            new ContentPackConfig
            {
                Id = "Armor_Common_1",
                Content = { new ArmorIdentity(ArmorName.Armor1, Grade.Common, 1) }
            },
        };

        /// <summary>
        /// Gets the configuration of available roster products.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<RosterProductConfig> RosterProductsConfig { get; set; } = new List<RosterProductConfig>()
        {
            new RosterProductConfig
            {
                Id = "Norris_Uncommon",
                Price = Price.CreateGold(1000),
                ContentPackId = "Norris_Uncommon_1",
                PlayerLevelRequired = 1,
            },
            new RosterProductConfig
            {
                Id = "Rock_Legendary",
                Price = Price.CreateRealCurrency("com.pocket.shooter.rock"),
                ContentPackId = "Rock_Legendary_1",
                PlayerLevelRequired = 2,
            },
            new RosterProductConfig
            {
                Id = "Minigun_Uncommon",
                Price = Price.CreateGold(10),
                ContentPackId = "Minigun_Uncommon_1",
                PlayerLevelRequired = 1,
            },
            new RosterProductConfig
            {
                Id = "Helmet1_Common",
                Price = Price.CreateCash(10),
                ContentPackId = "Helmet1_Common_1",
                PlayerLevelRequired = 1,
            },
            new RosterProductConfig
            {
                Id = "Armor1_Common",
                Price = Price.CreateCash(10),
                ContentPackId = "Armor1_Common_1",
                PlayerLevelRequired = 1,
            },
        };

        /// <summary>
        /// Gets the configuration of available shop products.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<ShopProductConfig> ShopProductsConfig { get; set; } = new List<ShopProductConfig>()
        {
            new ShopProductConfig
            {
                Id = "Somecoins",
                Price = Price.CreateRealCurrency("com.pocket.shooter.somecoins"),
                ContentPackId = "Gold_1000",
                Category = new List<ShopCategory> {ShopCategory.Soft},
            }
        };

        /// <summary>
        /// Gets or sets the configuration of grades.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(5)]
        public IList<GradeConfig> GradesConfig { get; set; } = new List<GradeConfig>()
        {
            new GradeConfig
            {
                Grade = Grade.Common,
                MaxLevel = 10,
            },
            new GradeConfig
            {
                Grade = Grade.Uncommon,
                MaxLevel = 20,
            },
            new GradeConfig
            {
                Grade = Grade.Rare,
                MaxLevel = 30,
            },
            new GradeConfig
            {
                Grade = Grade.Epic,
                MaxLevel = 40,
            },
            new GradeConfig
            {
                Grade = Grade.Legendary,
                MaxLevel = 50,
            }
        };

        /// <summary>
        /// Gets or sets the configuration of trooper classes.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(byte.MaxValue)]
        public IList<TrooperClassConfig> TrooperClassesConfig { get; set; } = new List<TrooperClassConfig>()
        {
            new TrooperClassConfig
            {
                Class = TrooperClass.Rambo,
                Skills = {SkillName.ExtraDamage, SkillName.ShockWaveJump, SkillName.StealthDash, SkillName.Grenade, SkillName.MedKit}
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Sniper,
                Skills = {SkillName.Immortality, SkillName.RocketJump, SkillName.Invisibility, SkillName.Grenade, SkillName.MedKit}
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Scout,
                Skills = {SkillName.StunningHit, SkillName.Heal, SkillName.Sprint, SkillName.Grenade, SkillName.MedKit}
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Spy,
                Skills = {SkillName.ControlFreak, SkillName.ShockWave, SkillName.StealthSprint, SkillName.Grenade, SkillName.MedKit}
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Norris,
                Skills = {SkillName.LifeDrain, SkillName.Dive, SkillName.RocketJump, SkillName.Grenade, SkillName.MedKit}
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Neo,
                Skills = {SkillName.RegenerationOnKill, SkillName.Lifesteal, SkillName.DoubleStealthDash, SkillName.Grenade, SkillName.MedKit }
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Statham,
                Skills = {SkillName.RootingHit, SkillName.InstantReload, SkillName.RocketJump, SkillName.Grenade, SkillName.MedKit}
            },
            new TrooperClassConfig
            {
                Class = TrooperClass.Rock,
                Skills = {SkillName.Rage, SkillName.Heal, SkillName.Lucky, SkillName.Grenade, SkillName.MedKit}
            },
        };

        /// <summary>
        /// Gets or sets the configuration of trooper levels.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<TrooperLevelConfig> TrooperLevelsConfig { get; set; } = new List<TrooperLevelConfig>()
        {
            new TrooperLevelConfig
            {
                Class = TrooperClass.Neo,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                RegularDuration = TimeSpan.FromMinutes(10),
                Stats  = ItemStats.CreateForTrooper(10, 10, 2000, 500, 5.0),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Rock,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 3000, 500, 3.0),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Statham,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 1800, 500, 3.0),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Norris,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 1950, 500, 4.5),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Spy,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 2000, 500, 5.0),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Scout,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 1900, 500, 4.0),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Sniper,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 1850, 500, 3.5),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Rambo,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats  = ItemStats.CreateForTrooper(10, 10, 1900, 500, 4.0),
            },
        };

        /// <summary>
        /// Gets or sets the configuration of trooper grades.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<TrooperGradeConfig> TrooperGradesConfig { get; set; } = new List<TrooperGradeConfig>()
        {
            new TrooperGradeConfig
            {
                Class = TrooperClass.Neo,
                Grade = Grade.Rare,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Rambo,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Scout,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Sniper,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Spy,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Norris,
                Grade = Grade.Uncommon,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Statham,
                Grade = Grade.Epic,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Rock,
                Grade = Grade.Legendary,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
        };

        /// <summary>
        /// Gets or sets the configuration of weapon levels.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<WeaponLevelConfig> WeaponLevelsConfig { get; set; } = new List<WeaponLevelConfig>()
        {
            new WeaponLevelConfig()
            {
                Name = WeaponName.Knife,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 1, 0, 0),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Katana,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 1, 0, 0),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.M16A4,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 15, 40, 1.2),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.SVD,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 50, 6, 1.2),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.SawedOff,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 10, 4, 1.2),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Minigun,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 20, 100, 1.2),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Remington,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 10, 8, 1.2),
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Barret,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForWeapon(10, 10, 50, 4, 1.2),
            },
        };

        /// <summary>
        /// Gets or sets the configuration of weapon grades.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<WeaponGradeConfig> WeaponGradesConfig { get; set; } = new List<WeaponGradeConfig>()
        {
            new WeaponGradeConfig()
            {
                Name = WeaponName.Knife,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Katana,
                Grade = Grade.Rare,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.M16A4,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.SVD,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.SawedOff,
                Grade = Grade.Uncommon,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Minigun,
                Grade = Grade.Legendary,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Remington,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Barret,
                Grade = Grade.Epic,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateEmpty(),
            },
        };

        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<HelmetLevelConfig> HelmetLevelsConfig { get; set; } = new List<HelmetLevelConfig>()
        {
            new HelmetLevelConfig
            {
                Name = HelmetName.Helmet1,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForHelmet(10, 10),
            }
        };

        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<HelmetGradeConfig> HelmetGradesConfig { get; set; } = new List<HelmetGradeConfig>()
        {
            new HelmetGradeConfig
            {
                Grade = Grade.Common,
                Name = HelmetName.Helmet1,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateForHelmet(10, 10),
            }
        };

        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<ArmorLevelConfig> ArmorLevelsConfig { get; set; } = new List<ArmorLevelConfig>()
        {
            new ArmorLevelConfig
            {
                Name = ArmorName.Armor1,
                Level = 1,
                InstantPrice = Price.CreateGold(10),
                RegularPrice = Price.CreateCash(100),
                Stats = ItemStats.CreateForArmor(10, 10),
            }
        };

        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<ArmorGradeConfig> ArmorGradesConfig { get; set; } = new List<ArmorGradeConfig>()
        {
            new ArmorGradeConfig
            {
                Name = ArmorName.Armor1,
                Grade = Grade.Common,
                InstantPrice = Price.CreateGold(10),
                Stats = ItemStats.CreateForArmor(10, 10),
            }
        };

        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<OfferPopupConfig> OfferPopupConfig { get; set; } = new List<OfferPopupConfig>()
        {
            new OfferPopupConfig
            {
                OfferProductId = "product_t2_starter_hero_20",
                AppearanceChance = 1f,
                Discount = 50,
            }
        };

        /// <summary>
        /// Builds an indexes for this configuration for fast lookup.
        /// </summary>
        public void BuildIndexes()
        {
            this.trooperLevelConfigLookup = TrooperLevelsConfig.ToDictionary(_ => (_.Class, _.Level));
            this.trooperGradeConfigLookup = TrooperGradesConfig.ToDictionary(_ => (_.Class, _.Grade));

            this.weaponLevelConfigLookup = WeaponLevelsConfig.ToDictionary(_ => (_.Name, _.Level));
            this.weaponGradeConfigLookup = WeaponGradesConfig.ToDictionary(_ => (_.Name, _.Grade));

            this.helmetLevelConfigLookup = HelmetLevelsConfig.ToDictionary(_ => (_.Name, _.Level));
            this.helmetGradeConfigLookup = HelmetGradesConfig.ToDictionary(_ => (_.Name, _.Grade));

            this.armorLevelConfigLookup = ArmorLevelsConfig.ToDictionary(_ => (_.Name, _.Level));
            this.armorGradeConfigLookup = ArmorGradesConfig.ToDictionary(_ => (_.Name, _.Grade));
        }

        /// <summary>
        /// Gets a trooper level configuration.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="level">The trooper's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public TrooperLevelConfig GetTrooperLevelConfig(TrooperClass trooperClass, int level)
        {
            TrooperLevelConfig levelConfig;

            if (trooperLevelConfigLookup != null)
            {
                trooperLevelConfigLookup.TryGetValue((trooperClass, level), out levelConfig);
            }
            else
            {
                levelConfig =
                    TrooperLevelsConfig
                    .SingleOrDefault(item => item.Class == trooperClass && item.Level == level);
            }

            if (levelConfig == null)
            {
                throw new ConfigurationException($"Can't find trooper level config for trooper {trooperClass} and level {level}");
            }

            return levelConfig;
        }

        /// <summary>
        /// Gets a trooper grade configuration.
        /// </summary>
        /// <param name="trooperClass">The trooper's class.</param>
        /// <param name="grade">The trooper's grade.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public TrooperGradeConfig GetTrooperGradeConfig(TrooperClass trooperClass, Grade grade)
        {
            TrooperGradeConfig gradeConfig;

            if (trooperGradeConfigLookup != null)
            {
                trooperGradeConfigLookup.TryGetValue((trooperClass, grade), out gradeConfig);
            }
            else
            {
                gradeConfig =
                    TrooperGradesConfig
                    .SingleOrDefault(item => item.Class == trooperClass && item.Grade == grade);
            }

            if (gradeConfig == null)
            {
                throw new ConfigurationException($"Can't find trooper grade config for trooper {trooperClass} and grade {grade}");
            }

            return gradeConfig;
        }

        /// <summary>
        /// Gets a weapon level configuration.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="level">The weapon's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public WeaponLevelConfig GetWeaponLevelConfig(WeaponName weaponName, int level)
        {
            WeaponLevelConfig levelConfig;

            if (weaponLevelConfigLookup != null)
            {
                weaponLevelConfigLookup.TryGetValue((weaponName, level), out levelConfig);
            }
            else
            {
                levelConfig =
                    WeaponLevelsConfig
                    .SingleOrDefault(item => item.Name == weaponName && item.Level == level);
            }

            if (levelConfig == null)
            {
                throw new ConfigurationException($"Can't find weapon level config for weapon {weaponName} and level {level}");
            }

            return levelConfig;
        }

        /// <summary>
        /// Gets a weapon grade configuration.
        /// </summary>
        /// <param name="weaponName">The weapon's name.</param>
        /// <param name="level">The weapon's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public WeaponGradeConfig GetWeaponGradeConfig(WeaponName weaponName, Grade grade)
        {
            WeaponGradeConfig gradeConfig;

            if (weaponGradeConfigLookup != null)
            {
                weaponGradeConfigLookup.TryGetValue((weaponName, grade), out gradeConfig);
            }
            else
            {
                gradeConfig =
                    WeaponGradesConfig
                    .SingleOrDefault(item => item.Name == weaponName && item.Grade == grade);
            }

            if (gradeConfig == null)
            {
                throw new ConfigurationException($"Can't find weapon grade config for weapon {weaponName} and grade {grade}");
            }

            return gradeConfig;
        }

        /// <summary>
        /// Gets a helmet level configuration.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="level">The helmet's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public HelmetLevelConfig GetHelmetLevelConfig(HelmetName helmetName, int level)
        {
            HelmetLevelConfig levelConfig;

            if (helmetLevelConfigLookup != null)
            {
                helmetLevelConfigLookup.TryGetValue((helmetName, level), out levelConfig);
            }
            else
            {
                levelConfig =
                    HelmetLevelsConfig
                    .SingleOrDefault(item => item.Name == helmetName && item.Level == level);
            }

            if (levelConfig == null)
            {
                throw new ConfigurationException($"Can't find helmet level config for helmet {helmetName} and level {level}");
            }

            return levelConfig;
        }

        /// <summary>
        /// Gets a helmet grade configuration.
        /// </summary>
        /// <param name="helmetName">The helmet's name.</param>
        /// <param name="level">The helmet's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public HelmetGradeConfig GetHelmetGradeConfig(HelmetName helmetName, Grade grade)
        {
            HelmetGradeConfig gradeConfig;

            if (helmetGradeConfigLookup != null)
            {
                helmetGradeConfigLookup.TryGetValue((helmetName, grade), out gradeConfig);
            }
            else
            {
                gradeConfig =
                    HelmetGradesConfig
                    .SingleOrDefault(item => item.Name == helmetName && item.Grade == grade);
            }

            if (gradeConfig == null)
            {
                throw new ConfigurationException($"Can't find helmet grade config for helmet {helmetName} and grade {grade}");
            }

            return gradeConfig;
        }

        /// <summary>
        /// Gets an armor level configuration.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="level">The armor's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public ArmorLevelConfig GetArmorLevelConfig(ArmorName armorName, int level)
        {
            ArmorLevelConfig levelConfig;

            if (armorLevelConfigLookup != null)
            {
                armorLevelConfigLookup.TryGetValue((armorName, level), out levelConfig);
            }
            else
            {
                levelConfig =
                    ArmorLevelsConfig
                    .SingleOrDefault(item => item.Name == armorName && item.Level == level);
            }

            if (levelConfig == null)
            {
                throw new ConfigurationException($"Can't find armor level config for armor {armorName} and level {level}");
            }

            return levelConfig;
        }

        /// <summary>
        /// Gets an armor grade configuration.
        /// </summary>
        /// <param name="armorName">The armor's name.</param>
        /// <param name="level">The armor's level.</param>
        /// <exception cref="ConfigurationException">If a configuration is not found.</exception>
        public ArmorGradeConfig GetArmorGradeConfig(ArmorName armorName, Grade grade)
        {
            ArmorGradeConfig gradeConfig;

            if (armorGradeConfigLookup != null)
            {
                armorGradeConfigLookup.TryGetValue((armorName, grade), out gradeConfig);
            }
            else
            {
                gradeConfig =
                    ArmorGradesConfig
                    .SingleOrDefault(item => item.Name == armorName && item.Grade == grade);
            }

            if (gradeConfig == null)
            {
                throw new ConfigurationException($"Can't find armor grade config for armor {armorName} and grade {grade}");
            }

            return gradeConfig;
        }
    }
}
