using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Lobby;
using Heyworks.PocketShooter.UI.Lobby.Presenters;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class AccountLevelUpPopup : BaseScreen
    {
        [SerializeField]
        private AcquiredContentCard cardPrefab;
        [SerializeField]
        private RectTransform cardsRoot;
        [SerializeField]
        private IconsFactory iconsFactory;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Button spaceCloseButton;
        [SerializeField]
        private Text titleLabel;
        [SerializeField]
        private Text levelFromLabel;
        [SerializeField]
        private Text levelToLabel;

        private void OnEnable()
        {
            closeButton.onClick.AddListener(CloseButtonOnClicked);
            spaceCloseButton.onClick.AddListener(CloseButtonOnClicked);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(CloseButtonOnClicked);
            spaceCloseButton.onClick.RemoveListener(CloseButtonOnClicked);
        }

        public void Setup(IEnumerable<IContentIdentity> content, int levelFrom, int levelTo)
        {
            levelFromLabel.text = levelFrom.ToString();
            levelToLabel.text = levelTo.ToString();

            foreach (Transform trans in cardsRoot)
            {
                Destroy(trans.gameObject);
            }

            int count = 0;
            foreach (IContentIdentity contentIdentity in content)
            {
                AcquiredContentCard card = Instantiate(cardPrefab, cardsRoot);
                var presenter = new AcquireConfirmationCardPresenter(card, iconsFactory);
                presenter.Setup(contentIdentity);

                AddDisposablePresenter(presenter);

                if (++count >= 3)
                {
                    break;
                }
            }
        }

        private void CloseButtonOnClicked()
        {
            Hide();
        }
    }
}
