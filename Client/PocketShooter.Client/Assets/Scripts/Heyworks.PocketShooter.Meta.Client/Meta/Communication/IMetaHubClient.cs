using System.Threading.Tasks;
using UniRx.Async;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMetaHubClient
    {
        ValueTask ConnectAsync();

        ValueTask DisconnectAsync();
    }
}