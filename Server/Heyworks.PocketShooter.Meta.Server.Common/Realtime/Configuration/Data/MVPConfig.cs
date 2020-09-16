using System.ComponentModel;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class MVPConfig
    {
        [DefaultValueAttribute(10.0)]
        public double KillsFactor { get; set; } = 10.0;

        [DefaultValueAttribute(1.0)]
        public double DeathsFactor { get; set; } = 1.0;
    }
}
