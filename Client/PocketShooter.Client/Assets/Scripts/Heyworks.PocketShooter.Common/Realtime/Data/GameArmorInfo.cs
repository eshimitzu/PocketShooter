namespace Heyworks.PocketShooter.Realtime.Data
{
    public class GameArmorInfo
    {
        public GameArmorInfo(float impact, float damageFactor)
        {
            Impact = impact;
            DamageFactor = damageFactor;
        }

        public float Impact { get; private set; }

        public float DamageFactor { get; private set; }
    }
}