using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;

namespace Heyworks.PocketShooter.Realtime
{
    public class BotSimulationData
    {
        public PlayerInfo BotInfo { get; }

        public Game Context { get; }

        public IClientSimulation ClientSimulation { get; }

        public INetworkSimulation NetworkSimulation { get; }

        public BotSimulationData(PlayerInfo botInfo, Game context, IClientSimulation clientSimulation, INetworkSimulation networkSimulation)
        {
            BotInfo = botInfo;
            Context = context;
            ClientSimulation = clientSimulation;
            NetworkSimulation = networkSimulation;
        }
    }
}