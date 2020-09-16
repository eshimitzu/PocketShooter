using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameContentGrain : IGrainWithGuidKey
    {
        Task ApplyContent(Immutable<IEnumerable<IContentIdentity>> content);
    }
}
