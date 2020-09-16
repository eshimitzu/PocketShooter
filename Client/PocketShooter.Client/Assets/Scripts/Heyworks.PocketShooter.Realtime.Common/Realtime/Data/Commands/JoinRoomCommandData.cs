namespace Heyworks.PocketShooter.Realtime.Data
{
    public readonly struct JoinRoomCommandData : IServiceCommandData
    {
        public JoinRoomCommandData(PlayerId playerId)
        {
            PlayerId = playerId;
        }

        public readonly PlayerId PlayerId;
    }
}