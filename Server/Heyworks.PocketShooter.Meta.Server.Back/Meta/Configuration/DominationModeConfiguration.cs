using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class DominationModeConfiguration : IDominationModeConfiguration
    {
        private readonly DominationModeConfig modeConfig;

        public DominationModeConfiguration(DominationModeConfig modeConfig)
        {
            this.modeConfig = modeConfig;
        }

        public int MaxPlayers => modeConfig.MaxPlayers;

        public double CalculateMVPRating(int killsCount, int deathsCount) =>
            (killsCount * modeConfig.MVPConfig.KillsFactor) - (deathsCount * modeConfig.MVPConfig.DeathsFactor);
    }
}
