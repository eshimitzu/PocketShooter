using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public interface INetworkService
    {
        void QueueCommand(SimulationCommandData command);

        void QueueCommand(IServiceCommandData command);

        void Send();

        void Receive();

        void AddPing(SimulationMetaCommandData pingCommandData, int possiblyStillAcceptedTick);
    }
}