using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameRoomsProviderGrain : IGrainWithGuidKey
    {
        Task<GameRoomData> RentRoom(MatchType matchType, MapNames map);
    }
}
