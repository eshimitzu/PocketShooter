namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents interface for providing information about ticks.
    /// </summary>
    public interface ITickProvider
    {
        /// <summary>
        /// Gets the local player tick.
        /// </summary>
        int Tick { get; }

        /// <summary>
        /// Gets the world tick.
        /// </summary>
        int WorldTick { get; }

        /// <summary>
        /// Gets the world tick fraction.
        /// </summary>
        float WorldTickFraction { get; }

        /// <summary>
        /// If the world switch to the new tick in this update.
        /// </summary>
        bool WorldTicked { get; }

        /// <summary>
        /// The tick which game referencing now.
        /// </summary>
        int LastUsedWorldTick { get; }

        /// <summary>
        /// How many tick elapsed from given tick
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns>Elapsed float.</returns>
        float ElapsedWorldTicks(float tick);
    }
}
