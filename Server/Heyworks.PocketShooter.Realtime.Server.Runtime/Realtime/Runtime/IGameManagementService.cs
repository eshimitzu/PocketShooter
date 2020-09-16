using System.Threading;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Realtime.Runtime
{
    public interface IGameManagementService
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        void Dispose();
    }
}
