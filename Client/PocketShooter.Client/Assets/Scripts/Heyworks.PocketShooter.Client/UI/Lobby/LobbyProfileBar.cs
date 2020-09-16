using System;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyProfileBar : MonoBehaviour
    {
        [SerializeField]
        private LobbyProfileView profileView;

        [SerializeField]
        private Button settingsButton;

        [SerializeField]
        private Text goldLabel;

        [SerializeField]
        private Text bucksLabel;

        [SerializeField]
        private Button cashButton;

        [SerializeField]
        private Button goldButton;

        [SerializeField]
        private Button storeButton;

        [SerializeField]
        private Button fightButton;

        [SerializeField]
        private Button backButton;

        public LobbyProfileView ProfileView => profileView;

        public Button SettingsButton => settingsButton;

        public Text GoldLabel => goldLabel;

        public Text BucksLabel => bucksLabel;

        public Button FightButton => fightButton;

        public Button BackButton => backButton;

        public event Action OnFightClick;

        public event Action OnBackClick;

        public event Action OnSettingsClick;

        public event Action<ShopCategory> OnStoreClick;

        private void OnEnable()
        {
            fightButton.onClick.AddListener(FightButtonOnClick);
            backButton.onClick.AddListener(BackButtonOnClick);
            settingsButton.onClick.AddListener(SettingsButtonOnClick);
            storeButton.onClick.AddListener(StoreButtonClick);
            cashButton.onClick.AddListener(CashButtonClick);
            goldButton.onClick.AddListener(GoldButtonClick);
        }

        private void OnDisable()
        {
            storeButton.onClick.RemoveListener(StoreButtonClick);
            cashButton.onClick.RemoveListener(CashButtonClick);
            goldButton.onClick.RemoveListener(GoldButtonClick);
            fightButton.onClick.RemoveListener(FightButtonOnClick);
            backButton.onClick.RemoveListener(BackButtonOnClick);
            settingsButton.onClick.RemoveListener(SettingsButtonOnClick);
        }

        private void FightButtonOnClick()
        {
            OnFightClick?.Invoke();
        }

        private void BackButtonOnClick()
        {
            OnBackClick?.Invoke();
        }

        private void SettingsButtonOnClick()
        {
            OnSettingsClick?.Invoke();
            ScreensController.Instance.ShowPopup<SettingsPopup>();
        }

        private void StoreButtonClick() => OnStoreClick?.Invoke(ShopCategory.Offers);

        private void CashButtonClick() => OnStoreClick?.Invoke(ShopCategory.Soft);

        private void GoldButtonClick() => OnStoreClick?.Invoke(ShopCategory.Hard);
    }
}