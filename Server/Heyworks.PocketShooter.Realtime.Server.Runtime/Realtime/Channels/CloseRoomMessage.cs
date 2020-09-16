namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal class CloseRoomMessage : IHasRoomIdMessage
    {
        public CloseRoomMessage(RoomId roomId)
        {
            RoomId = roomId;
        }

        public RoomId RoomId { get; }
    }
}
