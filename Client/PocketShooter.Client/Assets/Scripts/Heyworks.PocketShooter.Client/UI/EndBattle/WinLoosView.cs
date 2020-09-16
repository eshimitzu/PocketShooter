using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.EndBattle
{
    public class WinLoosView : MonoBehaviour
    {
        [SerializeField]
        private Text winnerLabel = null;

        [SerializeField]
        private Image shineView;

        [SerializeField]
        private GameVisualSettings gameVisualSettings;

        public void ShowGameResult(Result result)
        {
            winnerLabel.gameObject.SetActive(true);
            winnerLabel.SetLocalizedText(LocKeys.GetResultKey(result));

            switch (result)
            {
                case Result.Win:
                    shineView.color = gameVisualSettings.FriendColor;
                    break;
                case Result.Lose:
                    shineView.color = gameVisualSettings.EnemyColor;
                    break;
                case Result.Draw:
                    shineView.enabled = false;
                    break;
            }
        }

        public enum Result
        {
            Win,
            Lose,
            Draw,
        }
    }
}