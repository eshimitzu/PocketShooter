using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class RealtimeConfig
    {
        [Required]
        public DominationModeConfig DominationModeConfig { get; set; } = new DominationModeConfig();

        [Required]
        public IList<DominationMapConfig> Maps { get; set; } =  new List<DominationMapConfig>()
        {
            new DominationMapConfig
            {
                MapName = MapNames.Mexico,
            },
            new DominationMapConfig
            {
                MapName = MapNames.Italy,
            },
            new DominationMapConfig
            {
                MapName = MapNames.Peru,
            },
        };

        /// <summary>
        /// Gets or sets the trooper levels configuration.
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
                Stats  = new TrooperStatsConfig(2000, 500, 5.0f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Rock,
                Level = 1,
                Stats  = new TrooperStatsConfig(3000, 500, 3.0f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Statham,
                Level = 1,
                Stats  = new TrooperStatsConfig(1800, 500, 3.0f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Norris,
                Level = 1,
                Stats  = new TrooperStatsConfig(1950, 500, 4.5f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Spy,
                Level = 1,
                Stats  = new TrooperStatsConfig(2000, 500, 5.0f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Scout,
                Level = 1,
                Stats  = new TrooperStatsConfig(1900, 500, 4.0f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Sniper,
                Level = 1,
                Stats  = new TrooperStatsConfig(1850, 500, 3.5f),
            },
            new TrooperLevelConfig
            {
                Class = TrooperClass.Rambo,
                Level = 1,
                Stats  = new TrooperStatsConfig(1900, 500, 4.0f),
            },
        };

        /// <summary>
        /// Gets or sets the trooper grades configuration.
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
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Rambo,
                Grade = Grade.Common,
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Scout,
                Grade = Grade.Common,
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Sniper,
                Grade = Grade.Common,
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Spy,
                Grade = Grade.Common,
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Norris,
                Grade = Grade.Uncommon,
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Statham,
                Grade = Grade.Epic,
                Stats = new TrooperStatsConfig(),
            },
            new TrooperGradeConfig
            {
                Class = TrooperClass.Rock,
                Grade = Grade.Legendary,
                Stats = new TrooperStatsConfig(),
            },
        };

        /// <summary>
        /// Gets or sets the weapon levels configuration.
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
                Stats = new MeleeWeaponStatsConfig
                {
                    HitZoneWidth = 1f,
                    HitZoneHeight = 1f,
                    Damage = 400f,
                    CriticalMultiplier = 2f,
                    Fraction = 1,
                    AutoAim = false,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Katana,
                Level = 1,
                Stats = new MeleeWeaponStatsConfig
                {
                    HitZoneWidth = 1f,
                    HitZoneHeight = 1f,
                    Damage = 500f,
                    CriticalMultiplier = 1.5f,
                    Fraction = 1,
                    AutoAim = false,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.M16A4,
                Level = 1,
                Stats = new WeaponStatsConfig
                {
                    Damage = 100f,
                    CriticalMultiplier = 1.3f,
                    Dispersion = 5f,
                    ReloadTimeMs = 2 * 1000,
                    Fraction = 1,
                    AutoAim = true,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.SVD,
                Level = 1,
                Stats = new WarmingUpWeaponStatsConfig
                {
                    Damage = 400f,
                    CriticalMultiplier = 1.4f,
                    ResetProgressOnShot = true,
                    WarmingSpeed = 0.003f,
                    CoolingSpeed = 0.001f,
                    ReloadTimeMs = 2 * 1000,
                    Fraction = 1,
                    AutoAim = true,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.SawedOff,
                Level = 1,
                Stats = new WeaponStatsConfig
                {
                    Damage = 1200f,
                    ReloadTimeMs = 2000,
                    Dispersion = 10f,
                    FractionDispersion = 15f,
                    Fraction = 10,
                    CriticalMultiplier = 1.1f,
                    AutoAim = true,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Minigun,
                Level = 1,
                Stats = new WarmingUpWeaponStatsConfig
                {
                    Damage = 50f,
                    CriticalMultiplier = 1.2f,
                    Dispersion = 5f,
                    ReloadTimeMs = 4 * 1000,
                    ResetProgressOnShot = false,
                    WarmingSpeed = 0.002f,
                    CoolingSpeed = 0.001f,
                    Fraction = 1,
                    AutoAim = true,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Remington,
                Level = 1,
                Stats = new WeaponStatsConfig
                {
                    Damage = 600f,
                    ReloadTimeMs = 4 * 1000,
                    Dispersion = 10f,
                    FractionDispersion = 10f,
                    Fraction = 10,
                    CriticalMultiplier = 1.1f,
                    AutoAim = true,
                }
            },
            new WeaponLevelConfig()
            {
                Name = WeaponName.Barret,
                Level = 1,
                Stats = new WarmingUpWeaponStatsConfig
                {
                    Damage = 700f,
                    CriticalMultiplier = 1.4f,
                    ResetProgressOnShot = true,
                    WarmingSpeed = 0.0015f,
                    CoolingSpeed = 0.0005f,
                    ReloadTimeMs = 4 * 1000,
                    Fraction = 1,
                    AutoAim = true,
                }
            },
        };

        /// <summary>
        /// Gets or sets the weapon grades configuration.
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
                Stats = new MeleeWeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Katana,
                Grade = Grade.Rare,
                Stats = new MeleeWeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.M16A4,
                Grade = Grade.Common,
                Stats = new WeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.SVD,
                Grade = Grade.Common,
                Stats = new WarmingUpWeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.SawedOff,
                Grade = Grade.Uncommon,
                Stats = new WeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Minigun,
                Grade = Grade.Legendary,
                Stats = new WarmingUpWeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Remington,
                Grade = Grade.Common,
                Stats = new WeaponStatsConfig(),
            },
            new WeaponGradeConfig()
            {
                Name = WeaponName.Barret,
                Grade = Grade.Epic,
                Stats = new WarmingUpWeaponStatsConfig(),
            },
        };

        /// <summary>
        /// Gets or sets an armor levels configuration.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<ArmorLevelConfig> ArmorLevelsConfig { get; set; } = new List<ArmorLevelConfig>()
        {
            new ArmorLevelConfig
            {
                Name = ArmorName.Armor1,
                Level = 1,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 1
                }
            },
            new ArmorLevelConfig
            {
                Name = ArmorName.Armor2,
                Level = 1,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 2
                }
            },
            new ArmorLevelConfig
            {
                Name = ArmorName.Armor3,
                Level = 1,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 3
                }
            },
            new ArmorLevelConfig
            {
                Name = ArmorName.Armor4,
                Level = 1,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 4
                }
            },
            new ArmorLevelConfig
            {
                Name = ArmorName.Armor5,
                Level = 1,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 5
                }
            }
        };

        /// <summary>
        /// Gets or sets an armor grades configuration.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<ArmorGradeConfig> ArmorGradesConfig { get; set; } = new List<ArmorGradeConfig>()
            {
            new ArmorGradeConfig
            {
                Name = ArmorName.Armor1,
                Grade = Grade.Common,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 1
                }
            },
            new ArmorGradeConfig
            {
                Name = ArmorName.Armor2,
                Grade = Grade.Uncommon,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 2
                }
            },
            new ArmorGradeConfig
            {
                Name = ArmorName.Armor3,
                Grade = Grade.Rare,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 3
                }
            },
            new ArmorGradeConfig
            {
                Name = ArmorName.Armor4,
                Grade = Grade.Epic,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 4
                }
            },
            new ArmorGradeConfig
            {
                Name = ArmorName.Armor5,
                Grade = Grade.Legendary,
                Stats = new ArmorStatsConfig
                {
                    MaxArmor = 5
                }
            }
        };

        /// <summary>
        /// Gets or sets a helmet levels configuration.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<HelmetLevelConfig> HelmetLevelsConfig { get; set; } = new List<HelmetLevelConfig>()
        {
            new HelmetLevelConfig
            {
                Name = HelmetName.Helmet1,
                Level = 1,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 1
                }
            },
            new HelmetLevelConfig
            {
                Name = HelmetName.Helmet2,
                Level = 1,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 2
                }
            },
            new HelmetLevelConfig
            {
                Name = HelmetName.Helmet3,
                Level = 1,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 3
                }
            },
            new HelmetLevelConfig
            {
                Name = HelmetName.Helmet4,
                Level = 1,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 4
                }
            },
            new HelmetLevelConfig
            {
                Name = HelmetName.Helmet5,
                Level = 1,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 5
                }
            }
        };

        /// <summary>
        /// Gets or sets a helmet levels configuration.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<HelmetGradeConfig> HelmetGradesConfig { get; set; } = new List<HelmetGradeConfig>()
        {
            new HelmetGradeConfig
            {
                Name = HelmetName.Helmet1,
                Grade = Grade.Common,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 1
                }
            },
            new HelmetGradeConfig
            {
                Name = HelmetName.Helmet2,
                Grade = Grade.Uncommon,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 2
                }
            },
            new HelmetGradeConfig
            {
                Name = HelmetName.Helmet3,
                Grade = Grade.Rare,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 3
                }
            },
            new HelmetGradeConfig
            {
                Name = HelmetName.Helmet4,
                Grade = Grade.Epic,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 4
                }
            },
            new HelmetGradeConfig
            {
                Name = HelmetName.Helmet5,
                Grade = Grade.Legendary,
                Stats = new HelmetStatsConfig
                {
                    MaxHealth = 5
                }
            }
        };

        /// <summary>
        /// Gets or sets the skill grades configuration.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<SkillGradeConfig> SkillGradesConfig { get; set; } = new List<SkillGradeConfig>()
        {
            new SkillGradeConfig
            {
                Name = SkillName.ControlFreak,
                Grade = Grade.Common,
                Stats = new ControlFreakSkillStatsConfig
                {
                    IncreaseDamagePercent = 100f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.MedKit,
                Grade = Grade.Common,
                Stats = new MedKitSkillStatsConfig
                {
                    CooldownTimeMs = 6 * 1000,
                    AvailablePerSpawn = 5,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.MedKit,
                Grade = Grade.Uncommon,
                Stats = new MedKitSkillStatsConfig
                {
                    CooldownTimeMs = 6 * 1000,
                    AvailablePerSpawn = 5,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.MedKit,
                Grade = Grade.Rare,
                Stats = new MedKitSkillStatsConfig
                {
                    CooldownTimeMs = 6 * 1000,
                    AvailablePerSpawn = 5,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.MedKit,
                Grade = Grade.Epic,
                Stats = new MedKitSkillStatsConfig
                {
                    CooldownTimeMs = 6 * 1000,
                    AvailablePerSpawn = 5,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.MedKit,
                Grade = Grade.Legendary,
                Stats = new MedKitSkillStatsConfig
                {
                    CooldownTimeMs = 6 * 1000,
                    AvailablePerSpawn = 5,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.ShockWave,
                Grade = Grade.Common,
                Stats = new AoESkillStatsConfig
                {
                    CooldownTimeMs = 20000,
                    ActiveTimeMs = 2000,
                    CastingTimeMs = 800,
                    Radius = 4,
                    Height = 2,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Grenade,
                Grade = Grade.Common,
                Stats = new GrenadeSkillStatsConfig
                {
                    CooldownTimeMs = 3 * 1000,
                    Damage = 750,
                    SplashDamage = 750,
                    ExplosionRadius = 2.5f,
                    CastingTimeMs = 1000,
                    AvailablePerSpawn = 3,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Grenade,
                Grade = Grade.Uncommon,
                Stats = new GrenadeSkillStatsConfig
                {
                    CooldownTimeMs = 3 * 1000,
                    Damage = 750,
                    SplashDamage = 750,
                    ExplosionRadius = 2.5f,
                    CastingTimeMs = 1000,
                    AvailablePerSpawn = 3,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Grenade,
                Grade = Grade.Rare,
                Stats = new GrenadeSkillStatsConfig
                {
                    CooldownTimeMs = 3 * 1000,
                    Damage = 750,
                    SplashDamage = 750,
                    ExplosionRadius = 2.5f,
                    CastingTimeMs = 1000,
                    AvailablePerSpawn = 3,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Grenade,
                Grade = Grade.Epic,
                Stats = new GrenadeSkillStatsConfig
                {
                    CooldownTimeMs = 3 * 1000,
                    Damage = 750,
                    SplashDamage = 750,
                    ExplosionRadius = 2.5f,
                    CastingTimeMs = 1000,
                    AvailablePerSpawn = 3,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Grenade,
                Grade = Grade.Legendary,
                Stats = new GrenadeSkillStatsConfig
                {
                    CooldownTimeMs = 3 * 1000,
                    Damage = 750,
                    SplashDamage = 750,
                    ExplosionRadius = 2.5f,
                    CastingTimeMs = 1000,
                    AvailablePerSpawn = 3,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Dive,
                Grade = Grade.Uncommon,
                Stats = new DiveSkillStatsConfig
                {
                    CooldownTimeMs = 20000,
                    ActiveTimeMs = 2000,
                    CastingTimeMs = 700,
                    Radius = 4,
                    Height = 2,
                    Damage = 750,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.ShockWaveJump,
                Grade = Grade.Common,
                Stats = new AoESkillStatsConfig
                {
                    CooldownTimeMs = 10000,
                   ActiveTimeMs = 2000,
                    CastingTimeMs = 670,
                    Radius = 4,
                    Height = 2,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RocketJump,
                Grade = Grade.Common,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 3000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RocketJump,
                Grade = Grade.Uncommon,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 3000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RocketJump,
                Grade = Grade.Rare,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 3000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RocketJump,
                Grade = Grade.Epic,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 3000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RocketJump,
                Grade = Grade.Legendary,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 3000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.StunningHit,
                Grade = Grade.Common,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 20000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RootingHit,
                Grade = Grade.Epic,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 10000,
                    ActiveTimeMs = 2000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Invisibility,
                Grade = Grade.Common,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 20000,
                    ActiveTimeMs = 5000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Lucky,
                Grade = Grade.Legendary,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 20000,
                    ActiveTimeMs = 4000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Lifesteal,
                Grade = Grade.Rare,
                Stats = new LifestealSkillStatsConfig
                {
                    ActiveTimeMs = 4000,
                    CooldownTimeMs = 15000,
                    Radius = 4,
                    Height = 2,
                    StealPeriodMs = 1000,
                    StealPercent = 0.1f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Immortality,
                Grade = Grade.Common,
                Stats = new SkillStatsConfig
                {
                    ActiveTimeMs = 3000,
                    CooldownTimeMs = 20000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.ExtraDamage,
                Grade = Grade.Common,
                Stats = new ExtraDamageSkillStatsConfig
                {
                    CooldownTimeMs = 10000,
                    DamagePercentOfMaxHealth = 30f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.LifeDrain,
                Grade = Grade.Uncommon,
                Stats = new LifeDrainSkillStatsConfig
                {
                    DrainPercent = 0.20f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Sprint,
                Grade = Grade.Common,
                Stats = new SprintSkillStatsConfig
                {
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 10000,
                    SpeedMultiplier = 1.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Sprint,
                Grade = Grade.Uncommon,
                Stats = new SprintSkillStatsConfig
                {
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 10000,
                    SpeedMultiplier = 1.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Sprint,
                Grade = Grade.Rare,
                Stats = new SprintSkillStatsConfig
                {
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 10000,
                    SpeedMultiplier = 1.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Sprint,
                Grade = Grade.Epic,
                Stats = new SprintSkillStatsConfig
                {
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 10000,
                    SpeedMultiplier = 1.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Sprint,
                Grade = Grade.Legendary,
                Stats = new SprintSkillStatsConfig
                {
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 10000,
                    SpeedMultiplier = 1.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.InstantReload,
                Grade = Grade.Epic,
                Stats = new SkillStatsConfig
                {
                    CooldownTimeMs = 15000,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Rage,
                Grade = Grade.Legendary,
                Stats = new RageSkillStatsConfig
                {
                     CooldownTimeMs = 0,
                     IncreaseDamageIntervalMs = 64,
                     DecreaseDamageIntervalMs = 64,
                     IncreaseDamagePercent = 1,
                     DecreaseDamagePercent = 1,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.RegenerationOnKill,
                Grade = Grade.Rare,
                Stats = new RegenerationOnKillSkillStatsConfig
                {
                    CooldownTimeMs = 15000,
                    RegenerationPercent = 0.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.StealthDash,
                Grade = Grade.Common,
                Stats = new StealthDashSkillStatsConfig
                {
                   // Stealth active time
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 15000,
                    CastingTimeMs = 350,
                    Length = 10,
                    Speed = 25,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.DoubleStealthDash,
                Grade = Grade.Rare,
                Stats = new StealthDashSkillStatsConfig
                {
                    // Stealth active time
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 20000,
                    CastingTimeMs = 350,
                    Length = 15,
                    Speed = 25,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.StealthSprint,
                Grade = Grade.Common,
                Stats = new StealthSprintSkillStatsConfig
                {
                    // Sprint(!) active time
                    ActiveTimeMs = 5000,
                    CooldownTimeMs = 20000,
                    StealthActiveTimeMs = 5000,
                    SpeedMultiplier = 1.5f,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Heal,
                Grade = Grade.Common,
                Stats = new HealSkillStatsConfig
                {
                    CooldownTimeMs = 20 * 1000,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Heal,
                Grade = Grade.Uncommon,
                Stats = new HealSkillStatsConfig
                {
                    CooldownTimeMs = 20 * 1000,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Heal,
                Grade = Grade.Rare,
                Stats = new HealSkillStatsConfig
                {
                    CooldownTimeMs = 20 * 1000,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Heal,
                Grade = Grade.Epic,
                Stats = new HealSkillStatsConfig
                {
                    CooldownTimeMs = 20 * 1000,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
            new SkillGradeConfig
            {
                Name = SkillName.Heal,
                Grade = Grade.Legendary,
                Stats = new HealSkillStatsConfig
                {
                    CooldownTimeMs = 20 * 1000,
                    HealingPercent = 0.15f,
                    HealIntervalMs = 1 * 1000,
                    HealIntervalsPerUsage = 4,
                }
            },
        };
    }
}
