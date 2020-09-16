namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IDominationModeConfiguration
    {
        double CalculateMVPRating(int killsCount, int deathsCount);

        int MaxPlayers { get; }
    }
}
