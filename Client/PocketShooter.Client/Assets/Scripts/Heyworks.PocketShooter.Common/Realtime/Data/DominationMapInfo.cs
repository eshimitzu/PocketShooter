using System.Collections.Generic;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class DominationMapInfo
    {
        public DominationMapInfo(TeamInfo[] teams, DominationZoneInfo[] zones)
        {
            Teams = teams;
            Zones = zones;
        }

        private DominationMapInfo() { }

        public TeamInfo[] Teams { get; private set; }

        public DominationZoneInfo[] Zones { get; private set; }
    }
}