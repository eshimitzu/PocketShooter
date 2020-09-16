using Heyworks.PocketShooter.Meta.Data;
using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task CreatePlayerGame(Immutable<CreatePlayerData> data);

        Task<ServerGameState> GetState();

        Task CheckRunnables();
    }
}
