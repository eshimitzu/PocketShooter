using Heyworks.PocketShooter.Meta.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    /// <summary>
    /// Contains configuration dependent on player's level.
    /// </summary>
    public class PlayerLevelConfig
    {
        /// <summary>
        /// Gets or sets player's level.
        /// </summary>
        [Range(1, short.MaxValue)]
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets an experience in this level.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int ExperienceInLevel { get; set; }

        /// <summary>
        /// Gets or sets the cash battle reward base value.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int CashBattleRewardBase { get; set; }

        /// <summary>
        /// Gets or sets the gold battle reward base value.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int GoldBattleRewardBase { get; set; }

        /// <summary>
        /// Gets or sets the experience battle reward base value.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int ExpBattleRewardBase { get; set; }

        /// <summary>
        /// Gets or sets the reward for this level.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<IContentIdentity> Reward { get; set; } = new List<IContentIdentity>();

        /// <summary>
        /// Gets or sets an interval for providing repeating rewards.
        /// </summary>
        [Required]
        public TimeSpan RepeatingRewardInterval { get; set; }

        /// <summary>
        /// Gets or sets a repeating provided reward.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(short.MaxValue)]
        public IList<IContentIdentity> RepeatingReward { get; set; } = new List<IContentIdentity>();
    }
}
