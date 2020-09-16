using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerArmor : ArmorBase
    {
        private readonly IArmorConfiguration armorConfiguration;

        public ServerArmor(ArmorState armorState, IArmorConfiguration armorConfiguration)
            : base(armorState, armorConfiguration)
        {
            this.armorConfiguration = armorConfiguration;
        }

        public int GetMaxArmorInfo() =>
            ArmorStatsConfig.Sum(
                armorConfiguration.GetGradeRealtimeStats(Name, Grade),
                armorConfiguration.GetLevelRealtimeStats(Name, Level)).MaxArmor;
    }
}
