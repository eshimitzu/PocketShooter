using Heyworks.PocketShooter.Realtime.Configuration.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class ServerGameConfig : GameConfig
    {
        /// <summary>
        /// Gets or sets the realtime configuration.
        /// </summary>
        [Required]
        public RealtimeConfig RealtimeConfig { get; set; } = new RealtimeConfig();

        /// <summary>
        /// Gets or sets the battle reward factors configuration.
        /// </summary>
        [Required]
        public BattleRewardFactorsConfig BattleRewardFactorsConfig { get; set; } = new BattleRewardFactorsConfig();

        [Required]
        public MatchMakingConfiguration MatchMaking {get; set; } = new MatchMakingConfiguration();

        [Required]
        [Description("Allows to tune bots power within some limits according player level for training")]
        public IList<BotsTrainConfig> BotsTrainsConfig { get; set; } = new List<BotsTrainConfig>
        {
            new BotsTrainConfig{ Level = 1, DamageFactorPercent = 0.5f, ProtectionFactorPercent = 0.5f, MaximalGrade = Grade.Rare},
            new BotsTrainConfig{ Level = 2, DamageFactorPercent = 0.6f, ProtectionFactorPercent = 0.6f, MaximalGrade = Grade.Epic},
            new BotsTrainConfig{ Level = 3, DamageFactorPercent = 0.7f, ProtectionFactorPercent = 0.7f, MaximalGrade = Grade.Legendary}
        };

        public IList<MapsSelectorConfig> MapSelectors { get; set; } = new List<MapsSelectorConfig>
        {
            new MapsSelectorConfig
            {
                Name = MapNames.Mexico,
            },
            new MapsSelectorConfig
            {
                Name = MapNames.Italy,
                StartLevel = 7,
            },
            new MapsSelectorConfig
            {
                Name = MapNames.Peru,
                EndLevel = 42
            },
        };        

        /// <summary>
        /// Gets or sets the configuration of rewards for positions in battle.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<BattlePositionRewardConfig> BattlePositionsRewardConfig { get; set; } = new List<BattlePositionRewardConfig>()
        {
            new BattlePositionRewardConfig
            {
                TeamPosition = 1,
                CashRewardWin = 50,
                CashRewardLose = 40,
                ExpRewardWin = 50,
                ExpRewardLose = 40,
                LearningMeterRewardWin = 50,
                LearningMeterRewardLose = 40,
            },
            new BattlePositionRewardConfig
            {
                TeamPosition = 2,
                CashRewardWin = 40,
                CashRewardLose = 29,
                ExpRewardWin = 40,
                ExpRewardLose = 29,
                LearningMeterRewardWin = 40,
                LearningMeterRewardLose = 29,
            },
            new BattlePositionRewardConfig
            {
                TeamPosition = 3,
                CashRewardWin = 33,
                CashRewardLose = 22,
                ExpRewardWin = 33,
                ExpRewardLose = 22,
                LearningMeterRewardWin = 33,
                LearningMeterRewardLose = 22,
            },
            new BattlePositionRewardConfig
            {
                TeamPosition = 4,
                CashRewardWin = 29,
                CashRewardLose = 17,
                ExpRewardWin = 29,
                ExpRewardLose = 17,
                LearningMeterRewardWin = 29,
                LearningMeterRewardLose = 17,
            },
            new BattlePositionRewardConfig
            {
                TeamPosition = 5,
                CashRewardWin = 25,
                CashRewardLose = 10,
                ExpRewardWin = 25,
                ExpRewardLose = 12,
                LearningMeterRewardWin = 25,
                LearningMeterRewardLose = 10,
            }
        };
    }

    public static class ServerGameConfigExtensions
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GameConfig"/> class.
        /// </summary>
        public static GameConfig ToGameConfig(this ServerGameConfig self)
        {
            return new GameConfig()
            {
                Version = self.Version,
                PlayerLevelsConfig = self.PlayerLevelsConfig,
                InAppPurchasesConfig = self.InAppPurchasesConfig,
                ContentPacksConfig = self.ContentPacksConfig,
                RosterProductsConfig = self.RosterProductsConfig,
                ShopProductsConfig = self.ShopProductsConfig,
                GradesConfig = self.GradesConfig,
                TrooperClassesConfig = self.TrooperClassesConfig,
                TrooperLevelsConfig = self.TrooperLevelsConfig,
                TrooperGradesConfig = self.TrooperGradesConfig,
                WeaponLevelsConfig = self.WeaponLevelsConfig,
                WeaponGradesConfig = self.WeaponGradesConfig,
                ArmorGradesConfig = self.ArmorGradesConfig,
                ArmorLevelsConfig = self.ArmorLevelsConfig,
                HelmetGradesConfig = self.HelmetGradesConfig,
                HelmetLevelsConfig = self.HelmetLevelsConfig,
                OfferPopupConfig = self.OfferPopupConfig,
            };
        }
    }
}
