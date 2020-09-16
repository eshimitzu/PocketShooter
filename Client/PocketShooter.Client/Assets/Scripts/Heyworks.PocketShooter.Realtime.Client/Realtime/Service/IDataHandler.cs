using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public interface IDataHandler
    {
        void Handle(NetworkData data);
    }
}