using Heyworks.PocketShooter.Meta.Data;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMatchResultsProviderGrain : IGrainWithGuidKey
    {
        Task<MatchResultsData> GetMatchResults(Guid playerId);
    }
}
