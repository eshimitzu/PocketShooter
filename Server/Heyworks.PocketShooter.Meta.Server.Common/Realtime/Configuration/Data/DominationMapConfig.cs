using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class DominationMapConfig
    {
        private static readonly List<DominationZoneConfig> MexicoNightZoneConfigs = new List<DominationZoneConfig>
        {
            new DominationZoneConfig(0, 26.98f, -0.35f, 10.14f, 5f),
            new DominationZoneConfig(1, -0.94f, -0.35f, -1.85f, 5f),
            new DominationZoneConfig(2, -28.74f, -0.35f, -13.82f, 5f),
        };

        private static readonly List<TeamConfig> MexicoNightTeamConfigs = new List<TeamConfig>()
        {
            new TeamConfig(TeamNo.First, new List<SpawnPointConfig>()
            {
                new SpawnPointConfig(-20.78f, 3.98f, 30.06f, 180f),
                new SpawnPointConfig(-18.59f, 3.98f, 27.43f, 180f),
                new SpawnPointConfig(-20.67f, 3.98f, 27.68f, 180f),
                new SpawnPointConfig(-18.36f, 3.98f, 30.32f, 180f),
                new SpawnPointConfig(-20.15f, 1.31f, 31.67f, 180f),
                new SpawnPointConfig(-24.7f, 1.31f, 31.06f, 180f),
                new SpawnPointConfig(-21.6f, 1.31f, 28.23f, 180f),
            }),
            new TeamConfig(TeamNo.Second, new List<SpawnPointConfig>()
            {
                new SpawnPointConfig(19.16f, 3.98f, -33.76f, 0f),
                new SpawnPointConfig(17.02f, 3.98f, -33.68f, 0f),
                new SpawnPointConfig(19.12f, 3.98f, -30.61f, 0f),
                new SpawnPointConfig(16.60f, 3.98f, -30.71f, 0f),
                new SpawnPointConfig(22.76f, 1.31f, -34.99f, 0f),
                new SpawnPointConfig(18.27f, 1.31f, -32.18f, 0f),
                new SpawnPointConfig(18.36f, 1.31f, -34.92f, 0f),
            }),
        };

        [Description("Map these configuration is applied")]
        [Required]
        public MapNames MapName {get;set;} = MapNames.Mexico;

        [Required]
        [MinLength(1)]
        [MaxLength(7)]
        public IList<DominationZoneConfig> ZonesConfig { get; set; } = MexicoNightZoneConfigs;

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public IList<TeamConfig> TeamsConfig { get; set; } = MexicoNightTeamConfigs;
    }
}