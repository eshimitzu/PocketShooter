using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IPlayerContentGrain : IGrainWithGuidKey
    {
        Task AddResource(Immutable<ResourceIdentity> resource);

        Task AddExperience(int experience);
    }
}
