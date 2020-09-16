using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Ref.
    /// </summary>
    public struct SimulationRef : IRef<GameState>
    {
        private readonly IRefIndex<SimulationState> simulationStateRefIndex;
        private readonly int tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationRef"/> struct.
        /// </summary>
        public SimulationRef(IRefIndex<SimulationState> simulationStateRefIndex, int tick, bool isInterpolated = false)
        {
            this.simulationStateRefIndex = simulationStateRefIndex;
            this.tick = tick;
            IsInterpolated = isInterpolated;
        }

        /// <summary>Gets.</summary>
        public ref GameState Value => ref simulationStateRefIndex[tick].GameState;

        public Tick Tick => tick;

        // Was this state missed and interpolated or the original state from server.

        public bool IsInterpolated { get; }
    }
}