using Heyworks.PocketShooter.UI.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyProfileView : MonoBehaviour
    {
        [SerializeField]
        private Image profileIcon;

        [SerializeField]
        private Text profileLevelLabel;

        [SerializeField]
        private Text profileUsernameLabel;

        [SerializeField]
        private UIProgressBar profileLevelProgress;

        [SerializeField]
        private Text profileLevelProgressPercent;

        [SerializeField]
        private Button changeNameButton;

        public Image ProfileIcon => profileIcon;

        public Text ProfileLevelLabel => profileLevelLabel;

        public Text ProfileUsernameLabel => profileUsernameLabel;

        public UIProgressBar ProfileLevelProgress => profileLevelProgress;

        public Text ProfileLevelProgressPercent => profileLevelProgressPercent;

        public Button ChangeNameButton => changeNameButton;
    }
}