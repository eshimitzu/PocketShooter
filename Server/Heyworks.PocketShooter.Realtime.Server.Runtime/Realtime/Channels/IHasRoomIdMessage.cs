namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal interface IHasRoomIdMessage : IManagementMessage
    {
        RoomId RoomId { get; }
    }
}
