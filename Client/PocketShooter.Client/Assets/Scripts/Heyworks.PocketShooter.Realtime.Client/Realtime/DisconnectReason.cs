namespace Heyworks.PocketShooter.Realtime
{
    public enum DisconnectReason
    {
        ConnectionTimeout,
        JoinRoomTimeout,
        RoomIsFull,
        ServerConnectionError,
        InGameDisconnect,
        ManualDisconnect,
        GameEnd,
    }
}