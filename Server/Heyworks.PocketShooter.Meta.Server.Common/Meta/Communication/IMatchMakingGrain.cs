using System.Threading.Tasks;
using System.Net;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMatchMakingGrain : IGrainWithGuidKey
    {
        Task SubscribeToMatches(IMatchMakedObserver observer);

        Task FindRealtimeServer(PlayerId playerId, MatchRequest requestedMatch);

        Task<IPEndPoint[]> GetRealtimeServers();
    }
}
