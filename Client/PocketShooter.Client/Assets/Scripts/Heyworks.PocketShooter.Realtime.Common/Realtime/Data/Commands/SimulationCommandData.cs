namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class SimulationCommandData
    {
        public SimulationCommandData(int tick, IGameCommandData gameCommandData)
        {
            this.Tick = tick;
            this.GameCommandData = gameCommandData;
        }

        public int Tick { get; }

        public IGameCommandData GameCommandData { get; }
    }
}
