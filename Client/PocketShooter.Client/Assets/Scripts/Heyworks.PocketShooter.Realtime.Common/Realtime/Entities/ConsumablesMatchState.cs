using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class ConsumablesMatchState
    {
        private readonly Dictionary<EntityId, ConsumablesPlayerState> playerStats = new Dictionary<EntityId, ConsumablesPlayerState>();

        public IReadOnlyDictionary<EntityId, ConsumablesPlayerState> PlayerStats => playerStats;

        public void AddPlayer(EntityId trooperId, ConsumablesInfo consumablesInfo) =>
            playerStats.Add(trooperId, new ConsumablesPlayerState(consumablesInfo.TotalOffensives, consumablesInfo.TotalSupports));
    }
}