using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class MatchResultsData
    {
        public bool IsWinner { get; set; }

        public bool IsDraw { get; set; }

        public PlayerMatchStatsData Stats { get; set; }

        public PlayerReward Reward { get; set; }

        public IList<PlayerMatchStatsData> OtherPlayerStats { get; set; }
    }
}
