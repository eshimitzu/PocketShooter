using System.Collections.Generic;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class TeamInfo
    {
        public TeamInfo(TeamNo teamNo, SpawnPointInfo[] spawnPoints)
        {
            Number = teamNo;
            SpawnPoints = spawnPoints;
        }

        private TeamInfo() { }

        public TeamNo Number { get; private set; }

        public SpawnPointInfo[] SpawnPoints { get; private set; }
    }
}
