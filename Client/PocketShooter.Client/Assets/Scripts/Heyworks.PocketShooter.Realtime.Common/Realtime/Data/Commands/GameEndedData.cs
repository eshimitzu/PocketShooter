namespace Heyworks.PocketShooter.Realtime.Data
{
    public readonly struct GameEndedData
    {
        public GameEndedData(int tick)
        {
            Tick = tick;
        }

        public readonly int Tick;
    }
}