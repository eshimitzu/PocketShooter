using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public struct ZoneRef : IRef<ZoneState>
    {
        private readonly IRef<GameState> gameStateRef;
        private readonly byte zoneIndex;

        public ZoneRef(IRef<GameState> gameStateRef, byte zoneIndex)
        {
            this.gameStateRef = gameStateRef;
            this.zoneIndex = zoneIndex;
        }

        public ref ZoneState Value => ref gameStateRef.Value.Zones[zoneIndex];
    }
}