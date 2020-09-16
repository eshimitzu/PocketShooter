using System.Diagnostics;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents adjustable stop watch.
    /// </summary>
    /// <seealso cref="Heyworks.PocketShooter.Realtime.Simulation.IAdjustableStopwatch" />
    internal class AdjustableStopwatch : IAdjustableStopwatch
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly int maxAdjustment;

        private int initialAdjustment;
        private int adjustment;

        public AdjustableStopwatch(int maxAdjustment)
        {
            this.maxAdjustment = maxAdjustment;
        }

        public int Adjustment => adjustment;

        /// <summary>
        /// Gets the elapsed milliseconds.
        /// </summary>
        public long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds + initialAdjustment + adjustment;

        /// <inheritdoc cref="IAdjustableStopwatch"/>
        public long Start(int initialAdjustment)
        {
            this.initialAdjustment = initialAdjustment;

            stopwatch.Start();
            return ElapsedMilliseconds - initialAdjustment;
        }

        /// <summary>
        /// Adjusts this stopwatch.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void Adjust(int delta)
        {
            adjustment += delta;

            if (adjustment < -maxAdjustment)
            {
                adjustment = -maxAdjustment;
            }
            else if (adjustment > maxAdjustment)
            {
                adjustment = maxAdjustment;
            }
        }

        /// <summary>
        /// Resets the stopwatch adjustment (set to zero).
        /// </summary>
        public void ResetAdjustment()
        {
            adjustment = 0;
        }
    }
}