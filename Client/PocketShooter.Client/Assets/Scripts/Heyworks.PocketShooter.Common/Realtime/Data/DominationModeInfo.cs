namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class DominationModeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DominationModeInfo"/> class.
        /// </summary>
        /// <param name="maxPlayers">Maximum players in game.</param>
        /// <param name="respawnTime">Time to respawn.</param>
        /// <param name="timeplayersToCapture">The amount of "timeplayers" to capture the point.</param>
        /// <param name="checkInterval">The amount of time to wait before next zones state check.</param>
        /// <param name="winScore">The points needed to win.</param>
        /// <param name="gameDuration">The duration of the game.</param>
        /// <param name="gameArmorInfoInfo">The information about armor in game.</param>
        /// <param name="mapInfo">The information about map.</param>
        public DominationModeInfo(
            int maxPlayers,
            int respawnTime,
            int timeplayersToCapture,
            int checkInterval,
            int winScore,
            int gameDuration,
            GameArmorInfo gameArmorInfoInfo,
            DominationMapInfo mapInfo)
        {
            MaxPlayers = maxPlayers;
            RespawnTime = respawnTime;
            TimeplayersToCapture = timeplayersToCapture;
            CheckInterval = checkInterval;
            WinScore = winScore;
            GameDuration = gameDuration;
            GameArmorInfo = gameArmorInfoInfo;
            Map = mapInfo;
        }

        private DominationModeInfo() { }

        /// <summary>
        /// Gets the maximum players in game.
        /// </summary>
        public int MaxPlayers { get; private set; }

        /// <summary>
        /// Gets the time to respawn.
        /// </summary>
        public int RespawnTime { get; private set; }

        /// <summary>
        /// Gets the amount of "timeplayers" to capture the point (timeplayers = playersCount * seconds, real formula is a bit different).
        /// </summary>
        public int TimeplayersToCapture { get; private set; }

        /// <summary>
        /// Gets the amount of time to wait before next zones state check.
        /// </summary>
        public int CheckInterval { get; private set; }

        /// <summary>
        /// Gets the points to win.
        /// </summary>
        public int WinScore { get; private set; }

        /// <summary>
        /// Gets the duration of the game.
        /// </summary>
        public int GameDuration { get; private set; }

        public GameArmorInfo GameArmorInfo { get; set; }

        public DominationMapInfo Map { get; private set; }
    }
}
