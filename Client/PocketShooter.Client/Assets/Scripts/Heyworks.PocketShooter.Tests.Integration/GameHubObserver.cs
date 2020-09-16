using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Tests
{
    public class GameHubObserver : IGameHubObserver
    {
        public TaskCompletionSource<MatchMakingResultData> Maked = new TaskCompletionSource<MatchMakingResultData>();

        public TaskCompletionSource<ClientGameState> GotState = new TaskCompletionSource<ClientGameState>();

        public Task MatchMaked(MatchMakingResultData matchMakingResult)
        {
            Maked.SetResult(matchMakingResult);
            return Maked.Task;
        }

        public Task ReceiveGameState(ClientGameState gameState)
        {
            GotState.SetResult(gameState);
            return Maked.Task;
        }
    }
}