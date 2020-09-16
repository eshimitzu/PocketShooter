namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Game simulation.
    /// </summary>
    public interface ISimulation
    {
        /// <summary>
        /// Runs all system over state to produce new state for new tick.
        /// </summary>
        void Update();
    }
}