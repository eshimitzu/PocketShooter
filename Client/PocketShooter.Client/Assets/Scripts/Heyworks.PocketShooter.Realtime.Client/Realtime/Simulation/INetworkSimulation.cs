using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public interface INetworkSimulation : ITickEvents, ISimulation
    {
        void Start(int startTick, int serverTimestamp, in SimulationState initState);

        ITickProvider TickProvider { get; }
    }
}