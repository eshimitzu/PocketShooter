namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class TrooperStatsConfig
    {
        public TrooperStatsConfig()
        {
        }

        public TrooperStatsConfig(int maxHealth, int maxArmor, float speed)
        {
            MaxHealth = maxHealth;
            MaxArmor = maxArmor;
            Speed = speed;
        }

        public int MaxHealth { get; set; }

        public int MaxArmor { get; set; }

        public float Speed { get; set; }

        public static TrooperStatsConfig Sum(TrooperStatsConfig first, TrooperStatsConfig second)
        {
            return new TrooperStatsConfig
            {
                MaxHealth = first.MaxHealth + second.MaxHealth,
                MaxArmor = first.MaxArmor + second.MaxArmor,
                Speed = first.Speed + second.Speed,
            };
        }
    }
}
