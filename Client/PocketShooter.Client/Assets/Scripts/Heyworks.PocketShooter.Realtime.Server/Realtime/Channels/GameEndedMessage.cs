namespace Heyworks.PocketShooter.Realtime.Channels
{
    public sealed class GameEndedMessage : IMessage
    {
        public GameEndedMessage(int tick)
        {
            Tick = tick;
        }

        public int Tick { get; }
    }
}