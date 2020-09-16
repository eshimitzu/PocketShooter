namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public class Ticker : ITicker
    {
        public int Current
        {
            get;
            private set;
        }

        public void Tick() => Current++;
    }
}