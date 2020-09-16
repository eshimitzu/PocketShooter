using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.MessageProcessors
{
    public interface IPeer
    {
        void SendEvent(NetworkDataCode dataCode, byte[] data, SendOptions sendParameters);

        void SendEvent(NetworkDataCode dataCode, byte[] data, bool unreliable);

        int ConnectionId { get; }
    }
}