using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Meta.Communication;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal class StartGameMessage : IHasRoomIdMessage
    {
        public StartGameMessage(GameStartRequest request) => Request = request;

        public GameStartRequest Request { get; }

        public RoomId RoomId => Request.RoomId;
    }
}
