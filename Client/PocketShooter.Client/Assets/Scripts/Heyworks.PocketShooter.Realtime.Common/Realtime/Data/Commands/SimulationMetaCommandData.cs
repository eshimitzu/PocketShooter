namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Dummy command to hold connection and calculate network statistics.
    /// </summary>
    public readonly struct SimulationMetaCommandData
    {
        public SimulationMetaCommandData(int tick, int confirmedTick)
        {
            Tick = tick;
            ConfirmedTick = confirmedTick;
        }

        /// <summary>
        /// Current predicted client tick.
        /// </summary>
        public readonly int Tick;

        // TODO: tick should be bit mask over last received tick, not tick value, so we can make this non reliable and out of order

        /// <summary>
        /// Last confirmed tick to allow server to send correct baseline.
        /// </summary>
        public readonly int ConfirmedTick;
    }
}