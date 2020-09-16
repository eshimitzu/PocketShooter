using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    internal class EndGameSystem : IServerGameSystem, IServerInitializeSystem
    {
        private readonly ITicker ticker;
        private readonly int winScore;
        private readonly int gameDuration;

        public EndGameSystem(ITicker ticker, int winScore, int gameDuration)
        {
            this.ticker = ticker;
            this.winScore = winScore;
            this.gameDuration = gameDuration;
        }

        public void Initialize(IServerGame game)
        {
            game.EndTime = ticker.Current + gameDuration;
        }

        public bool Execute(IServerGame game)
        {
            if ((game.Team1.State.Score >= winScore || game.Team2.State.Score >= winScore)
                || ticker.Current >= game.EndTime)
            {
                game.IsEnded = true;

                game.MatchResult.WinnerTeam = game.Team1.State.Score > game.Team2.State.Score
                    ? game.Team1.State.Number
                    : game.Team1.State.Score < game.Team2.State.Score
                        ? game.Team2.State.Number
                        : TeamNo.None;
            }

            return true;
        }
    }
}