namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents interface for the adjustable stopwatch.
    /// </summary>
    public interface IAdjustableStopwatch
    {
        int Adjustment { get; }

        /// <summary>
        /// Gets the elapsed milliseconds.
        /// </summary>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// Starts the stopwatch.
        /// </summary>
        /// <param name="initialAdjustment">Initial adjustment.</param>
        /// <returns>Start time.</returns>
        long Start(int initialAdjustment);

        /// <summary>
        /// Adjusts this stopwatch.
        /// </summary>
        /// <param name="delta">The delta.</param>
        void Adjust(int delta);

        /// <summary>
        /// Resets the stopwatch adjustment (set to zero).
        /// </summary>
        void ResetAdjustment();
    }
}
