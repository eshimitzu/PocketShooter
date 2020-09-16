using System;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Contains states with interpolation for remotes.
    /// </summary>
    public interface IInterpolatingStateProvider<T> : IStateProvider<T>
    {
        /// <summary>
        /// Determines whether the state with given tick came from server or was interpolated.
        /// </summary>
        /// <param name="tick">Tick to check.</param>
        /// <returns>True if the state isn't interpolated.</returns>
        bool IsInterpolated(int tick);
    }
}