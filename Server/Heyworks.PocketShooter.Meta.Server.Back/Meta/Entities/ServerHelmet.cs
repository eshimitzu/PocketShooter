using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerHelmet : HelmetBase
    {
        private readonly IHelmetConfiguration helmetConfiguration;

        public ServerHelmet(HelmetState helmetState, IHelmetConfiguration helmetConfiguration)
            : base(helmetState, helmetConfiguration)
        {
            this.helmetConfiguration = helmetConfiguration;
        }

        public int GetMaxHealthInfo() =>
            HelmetStatsConfig.Sum(
                helmetConfiguration.GetGradeRealtimeStats(Name, Grade),
                helmetConfiguration.GetLevelRealtimeStats(Name, Level)).MaxHealth;
    }
}
