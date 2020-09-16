using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMatchResultsPublisherGrain : IGrainWithGuidKey
    {
        Task ApplyMatchResults(Immutable<IList<PlayerMatchResultsData>> resultsData);
    }
}
