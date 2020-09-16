namespace Heyworks.PocketShooter.UI
{
    public interface ILobbyRosterProvider
    {
        ILobbyRosterPresenter CurrentRoster { get; }
    }
}