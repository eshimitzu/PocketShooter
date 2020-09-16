using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class VersionInfoPopup : BaseScreen
    {
        [SerializeField]
        private Button acceptButton;

        [SerializeField]
        private Text titleLabel;

        [SerializeField]
        private Text descriptionLabel;

        [SerializeField]
        private DeepLinksConfig deepLinksConfig;

        private void OnEnable()
        {
            acceptButton.onClick.AddListener(AcceptButtonOnClicked);
        }

        private void OnDisable()
        {
            acceptButton.onClick.RemoveListener(AcceptButtonOnClicked);
        }

        private void AcceptButtonOnClicked()
        {
            Application.OpenURL(deepLinksConfig.PocketShooter);
        }
    }
}