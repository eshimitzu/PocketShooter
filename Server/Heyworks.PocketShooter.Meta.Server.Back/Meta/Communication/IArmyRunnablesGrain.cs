using System.Threading.Tasks;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IArmyRunnablesGrain : IGrainWithGuidKey
    {
        Task CheckRunnables();
    }
}
