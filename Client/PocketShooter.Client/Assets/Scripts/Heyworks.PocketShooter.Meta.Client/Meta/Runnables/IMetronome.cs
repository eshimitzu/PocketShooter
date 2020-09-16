using System;

namespace Heyworks.PocketShooter.Meta.Runnables
{
    /// <summary>
    /// Represents interface of an object which produces regular, metrical ticks.
    /// </summary>
    public interface IMetronome
    {
        /// <summary>
        /// Gets a tick interval in seconds.
        /// </summary>
        float TickIntervalSeconds
        {
            get;
        }

        /// <summary>
        /// Rises each time interval, specified in property TickIntervalSeconds.
        /// </summary>
        event Action Tick;
    }
}