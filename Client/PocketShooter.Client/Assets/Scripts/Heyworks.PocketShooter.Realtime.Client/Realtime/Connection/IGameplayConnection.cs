using System;

namespace Heyworks.PocketShooter.Realtime.Connection
{
    public interface IGameplayConnection : IConnection, ICommunication
    {
        event Action OnDisconnected;

        event Action OnDisconnectedByServer;
    }
}