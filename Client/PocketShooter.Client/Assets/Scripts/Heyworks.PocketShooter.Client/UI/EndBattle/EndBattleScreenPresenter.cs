using Heyworks.PocketShooter.UI.Core;

namespace Heyworks.PocketShooter.UI.EndBattle
{
    public class EndBattleScreenPresenter : IDisposablePresenter
    {
        public EndBattleScreenPresenter(WinLoosView winLoseView, bool isDraw, bool weWin)
        {
            if (isDraw)
            {
                winLoseView.ShowGameResult(WinLoosView.Result.Draw);
            }
            else
            {
                winLoseView.ShowGameResult(weWin ? WinLoosView.Result.Win : WinLoosView.Result.Lose);
            }
        }

        public void Dispose()
        {
        }
    }
}
