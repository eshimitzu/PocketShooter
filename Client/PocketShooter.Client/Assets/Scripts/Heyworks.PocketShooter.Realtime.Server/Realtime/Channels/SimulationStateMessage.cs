using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    /// <summary>
    /// Represents a simulation state message.
    /// </summary>
    internal sealed class SimulationStateMessage : IMessage
    {
        private readonly SimulationState? baseline;

        private readonly SimulationState updated;

        public ref readonly SimulationState? Baseline => ref baseline;

        public ref readonly SimulationState Updated => ref updated;

        public SimulationStateMessage(SimulationState? baseline, SimulationState updated)
        {
            this.baseline = baseline;
            this.updated = updated;
        }
    }
}
