using System.Net;
using System.Threading.Tasks;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMatchMakingObservableGrain : IGrainWithGuidKey
    {
        Task Subscribe(IMatchMakingObserver observer, IPEndPoint observerAddress);
    }
}
