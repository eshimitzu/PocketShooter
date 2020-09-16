using System.Threading.Tasks;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface ICheatsGrain : IGrainWithGuidKey
    {
        Task UnlockContent();

        Task AddResources(int cash, int gold);

        Task LevelUpPlayer();
    }
}
