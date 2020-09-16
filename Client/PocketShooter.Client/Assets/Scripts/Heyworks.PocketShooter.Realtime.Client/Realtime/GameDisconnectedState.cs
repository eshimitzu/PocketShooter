namespace Heyworks.PocketShooter.Realtime
{
    public class GameDisconnectedState : IGameState
    {
        public DisconnectReason DisconnectReason { get; private set; }

        public GameDisconnectedState(DisconnectReason disconnectReason)
        {
            DisconnectReason = disconnectReason;
        }

        public void Update()
        {
        }
    }
}