using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Server-side game interface.
    /// </summary>
    public interface IServerGame : IGame
    {
        IReadOnlyDictionary<EntityId, ServerPlayer> Players { get; }

        ServerPlayer GetServerPlayer(EntityId id);

        /// <summary>
        /// Gets the zones.
        /// </summary>
        IReadOnlyDictionary<byte, Zone> Zones { get; }

        /// <summary>
        /// Gets or sets a value indicating whether game is ended.
        /// </summary>
        bool IsEnded { get; set; }

        long EndTime { get; set; }

        DominationMatchResult MatchResult { get; }
            
        void RespawnTrooper(string nickname, EntityId id, TeamNo teamNo, TrooperInfo trooperInfo);
    }
}