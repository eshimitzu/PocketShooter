using System;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public interface ITickEvents
    {
        /// <summary>
        /// Gets observable that fires when simulation ticks. Has parameter number of ticks passed since last tick.
        /// </summary>
        IObservable<TickEvent> SimulationTick { get; }
    }

    public struct TickEvent
    {
    }
}