using Orleans.Concurrency;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMatchMakedObserver : IGrainObserver
    {
        // happens when there is match for user or group of users
        void OnMatchMaked(MatchMakingResultData result, Immutable<PlayerId[]> players);
    }
}