using System;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Lobby;
using Heyworks.PocketShooter.UI.Lobby.Presenters;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class UpgradeContentPopup : BaseScreen
    {
        [SerializeField]
        private UpgradeContentCard cardPrefab;
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

        public event Action<UpgradeContentPopup> Closed;

        public IContentIdentity ContentIdentity { get; private set; }

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

        public void Setup(IContentIdentity content, bool isGradeUp)
        {
            ContentIdentity = content;

            SetTitle(isGradeUp ? LocKeys.Evolution : LocKeys.LevelUp);

            foreach (Transform trans in cardsRoot)
            {
                Destroy(trans.gameObject);
            }

            var card = Instantiate(cardPrefab, cardsRoot);
            var presenter = new UpgradeContentCardPresenter(card, iconsFactory);
            presenter.Setup(content, isGradeUp);

            AddDisposablePresenter(presenter);
        }

        public void SetTitle(string key)
        {
            titleLabel.SetLocalizedText(key);
        }

        private void CloseButtonOnClicked()
        {
            Hide();

            if (ScreensController.Instance.CurrentScreen is LobbyScreen lobbyScreen)
            {
                lobbyScreen.SelectItem(ContentIdentity);
            }
        }
    }
}