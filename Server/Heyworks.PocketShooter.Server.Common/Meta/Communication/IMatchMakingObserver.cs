using Heyworks.PocketShooter.Realtime.Data;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IMatchMakingObserver : IGrainObserver
    {
        void StartGame(GameStartRequest request);

        void JoinServer(RoomId roomId, PlayerInfo playerInfo);

        // adds bot prototype to put into game according configurations and level of players in game
        void AddBot(RoomId roomId, PlayerInfo playerInfo);
    }
}
