namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class ArmorStatsConfig
    {
        /// <summary>
        /// Gets or sets the impact for max armor.
        /// </summary>
        public int MaxArmor { get; set; }

        public static ArmorStatsConfig Sum(ArmorStatsConfig first, ArmorStatsConfig second)
        {
            return new ArmorStatsConfig
            {
                MaxArmor = first.MaxArmor + second.MaxArmor,
            };
        }
    }
}
