namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Spawn bot's trooper command data.
    /// </summary>
    public readonly struct SpawnBotTrooperCommandData : IServiceCommandData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnBotTrooperCommandData"/> struct.
        /// </summary>
        /// <param name="botId">The bot id.</param>
        /// <param name="trooperClass">The trooper class.</param>
        public SpawnBotTrooperCommandData(BotId botId, TrooperClass trooperClass)
        {
            BotId = botId;
            TrooperClass = trooperClass;
        }

        /// <summary>
        /// Gets the bot's id.
        /// </summary>
        public readonly BotId BotId;

        /// <summary>
        /// Gets the trooper's class.
        /// </summary>
        public readonly TrooperClass TrooperClass;
    }
}
