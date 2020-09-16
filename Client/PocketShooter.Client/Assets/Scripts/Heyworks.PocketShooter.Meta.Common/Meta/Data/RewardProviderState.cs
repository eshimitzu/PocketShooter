using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class RewardProviderState
    {
        /// <summary>
        /// Gets or sets a timestamp when the reward was collected last time.
        /// </summary>
        public DateTime NextRewardAvailableAt { get; set; }
    }
}
