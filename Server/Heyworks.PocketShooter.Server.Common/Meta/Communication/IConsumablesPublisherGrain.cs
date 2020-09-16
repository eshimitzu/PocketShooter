using System.Threading.Tasks;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IConsumablesPublisherGrain : IGrainWithGuidKey
    {
        Task UpdateConsumables(int usedOffensives, int usedSupports);
    }
}
