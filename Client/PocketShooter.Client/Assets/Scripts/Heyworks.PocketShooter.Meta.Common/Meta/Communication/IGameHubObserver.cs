using Heyworks.PocketShooter.Meta.Data;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameHubObserver
    {
        Task MatchMaked(MatchMakingResultData matchMakingResult);

        Task ReceiveGameState(ClientGameState gameState);
    }
}