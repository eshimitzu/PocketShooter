using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Systems;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public interface IClientSimulation
    {
        Game Game { get; }

        void AddCommand(IGameCommandData gameCommand);

        void AddCommand(IServiceCommandData serviceCommand);
    }
}
