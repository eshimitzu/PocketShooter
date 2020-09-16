using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameRoomsPublisherGrain : IGrainWithGuidKey
    {
        Task PublishRoomData(Immutable<List<GameRoomData>> roomData);
    }
}