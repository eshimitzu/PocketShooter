using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents ticker for counting simulation ticks.
    /// </summary>
    public sealed class Ticker : ITicker
    {
        private readonly IAdjustableStopwatch stopwatch;
        private readonly int tickIntervalMs;
        private long lastTickTime;

        /// <summary>
        /// Gets the current tick. Updated on every Tick() method if more then tick interval passed since last tick.
        /// </summary>
        public int Current { get; private set; }

        /// <summary>
        /// Gets the fraction of tick passed since last tick.
        /// </summary>
        internal float Fraction => (float)(stopwatch.ElapsedMilliseconds - lastTickTime) / tickIntervalMs;

        internal float ElapsedTicks(float tick) =>
            (float)(stopwatch.ElapsedMilliseconds - lastTickTime) / tickIntervalMs + (Current - tick);

        /// <summary>
        /// Gets the actual number of ticks passed since ticker was started. ActualCurrent - Current > 1 if Tick() method was not called longer then tick interval.
        /// </summary>
        internal float ActualCurrent => Current + Fraction;

        internal int Adjustment => stopwatch.Adjustment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ticker" /> class.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        /// <param name="tickIntervalMs">The tick interval in milliseconds.</param>
        internal Ticker(IAdjustableStopwatch stopwatch, int tickIntervalMs)
        {
            Assert.IsNotNull(stopwatch);
            Assert.IsTrue(tickIntervalMs > 0 && tickIntervalMs < 10000);

            this.stopwatch = stopwatch;
            this.tickIntervalMs = tickIntervalMs;
        }

        /// <summary>
        /// Starts the ticker.
        /// </summary>
        internal void Start(int startTick, int serverDelayMs, int initialOffset)
        {
            Current = startTick;

            lastTickTime = stopwatch.Start(serverDelayMs);

            stopwatch.Adjust(initialOffset * tickIntervalMs);
        }

        /// <summary>
        /// Calculate how many ticks passed since last tick.
        /// </summary>
        /// <returns>Returns number of ticks passed. Can be zero or more.</returns>
        internal int Tick()
        {
            var time = stopwatch.ElapsedMilliseconds;
            var ticked = (time - lastTickTime) / tickIntervalMs;
            if (ticked > 0)
            {
                lastTickTime += ticked * tickIntervalMs;
                Current += (int)ticked;

                return (int)ticked;
            }

            return 0;
        }

        /// <summary>
        /// Adjusts this stopwatch.
        /// </summary>
        /// <param name="adjustment">Stopwatch adjustment.</param>
        internal void Adjust(int adjustment)
        {
            stopwatch.Adjust(adjustment);
        }

        #region Debug Only

        /// <summary>
        /// Set ticker to the specified tick and reset stopwatch adjustment.
        /// </summary>
        /// <param name="tick">The tick to set.</param>
        internal void Reset(int tick)
        {
            Current = tick;

            stopwatch.ResetAdjustment();
            lastTickTime = stopwatch.ElapsedMilliseconds;
        }

        #endregion
    }
}
