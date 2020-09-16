using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Lobby;
using Heyworks.PocketShooter.UI.Lobby.Presenters;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class AcquireConfirmationPopup : BaseScreen
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

        public void Setup(IEnumerable<IContentIdentity> content)
        {
            foreach (Transform trans in cardsRoot)
            {
                Destroy(trans.gameObject);
            }

            foreach (var contentIdentity in content)
            {
                var card = Instantiate(cardPrefab, cardsRoot);
                var presenter = new AcquireConfirmationCardPresenter(card, iconsFactory);
                presenter.Setup(contentIdentity);

                AddDisposablePresenter(presenter);
            }
        }

        public void SetTitle(string key)
        {
            titleLabel.SetLocalizedText(key);
        }

        private void CloseButtonOnClicked()
        {
            Hide();
        }
    }
}
