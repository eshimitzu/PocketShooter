namespace Heyworks.PocketShooter.Realtime.Channels
{
    public sealed class SpawnBotTrooperMessage : IServiceMessage
    {
        public SpawnBotTrooperMessage(BotId botId, TrooperClass trooperClass)
        {
            BotId = botId;
            TrooperClass = trooperClass;
        }

        public BotId BotId { get; }

        public TrooperClass TrooperClass { get; }
    }
}