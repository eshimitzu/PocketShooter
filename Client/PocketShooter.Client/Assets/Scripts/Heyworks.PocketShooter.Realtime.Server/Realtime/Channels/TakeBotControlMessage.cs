using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    public sealed class TakeBotControlMessage : IServiceMessage
    {
        public TakeBotControlMessage(PlayerId ownerId, PlayerInfo botInfo)
        {
            OwnerId = ownerId;
            BotInfo = botInfo;
        }

        public PlayerId OwnerId { get; }

        public PlayerInfo BotInfo { get; }
    }
}