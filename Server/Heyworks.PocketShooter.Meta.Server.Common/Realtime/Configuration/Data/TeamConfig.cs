using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class TeamConfig
    {
        public TeamConfig(TeamNo teamNo, IList<SpawnPointConfig> spawnPoints)
        {
            TeamNo = teamNo;
            SpawnPoints = spawnPoints;
        }

        public TeamConfig() { }

        public TeamNo TeamNo { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(7)]
        public IList<SpawnPointConfig> SpawnPoints { get; set; } = new List<SpawnPointConfig>();
    }
}
