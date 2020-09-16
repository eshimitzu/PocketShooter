using System.Collections.Generic;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class DominationMatchResult
    {
        private readonly Dictionary<EntityId, PlayerMatchStats> playerStats = new Dictionary<EntityId, PlayerMatchStats>();

        public IReadOnlyDictionary<EntityId, PlayerMatchStats> PlayerStats => playerStats;

        public TeamNo WinnerTeam { get; set; }

        public void AddPlayer(EntityId trooperId) => playerStats.Add(trooperId, new PlayerMatchStats());
    }
}