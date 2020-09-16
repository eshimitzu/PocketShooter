using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Singleton;
using Heyworks.PocketShooter.UI.Popups;
using Heyworks.PocketShooter.Utils;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.UI.Core
{
    /// <summary>
    /// Represents screens controller.
    /// </summary>
    public class ScreensController : Singleton<ScreensController>
    {
        private const float MaxPurchaseInitializationTime = 10f;

        [SerializeField]
        private List<BaseScreen> screens = null;

        [SerializeField]
        private List<BaseScreen> popups = null;

        private BaseScreen currentPopup;
        private BaseScreen currentScreen;
        private BaseScreen.Factory baseScreenFactory;

        public BaseScreen CurrentScreen => currentScreen;

        public BaseScreen CurrentView
        {
            get
            {
                return currentPopup != null ? currentPopup : CurrentScreen;
            }
        }

        /// <summary>
        /// Zenject initialization.
        /// </summary>
        /// <param name="factory">BaseScreen factory.</param>
        [Inject]
        public void Init(BaseScreen.Factory factory)
        {
            transform.SetAsFirstSibling();
            AssertUtils.NotNull(factory, nameof(factory));

            baseScreenFactory = factory;
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <typeparam name="T">The type of the screen.</typeparam>
        public T ShowScreen<T>()
            where T : BaseScreen
        {
            HideCurrentScreen();

            BaseScreen screenPrefab = screens.Find(bs => (bs.GetType() == typeof(T)));

            currentScreen = baseScreenFactory.Create(screenPrefab.gameObject);
            currentScreen.transform.SetParent(transform, false);
            currentScreen.transform.SetAsFirstSibling();

            return currentScreen.GetComponent<T>();
        }

        public T ShowPopup<T>()
            where T : BaseScreen
        {
            BaseScreen popupPrefab = popups.Find(bs => (bs.GetType() == typeof(T)));

            currentPopup = baseScreenFactory.Create(popupPrefab.gameObject);
            currentPopup.transform.SetParent(transform, false);

            return currentPopup.GetComponent<T>();
        }

        public void ShowMessageBox(string title, string text, PopupOptionInfo option1, PopupOptionInfo option2 = null, Action onCancel = null)
        {
            ShowPopup<MessageBoxPopup>().Setup(title, text, option1, option2, onCancel);
        }

        public void HideCurrentScreen()
        {
            if (currentScreen != null)
            {
                currentScreen.Hide();
                currentScreen = null;
            }
        }

        public async void ShowStoreWhenReadyAsync(MetaGame metaGame, ShopCategory categoryType)
        {
            ProgressHUD.Instance.Show();

            // wait until purchases initialized.
            var startInitializationTime = Time.time;
            await UniTask.WaitUntil(() => metaGame.Shop.ArePurchasesInitialized || Time.time > startInitializationTime + MaxPurchaseInitializationTime);

            ProgressHUD.Instance.Hide();

            var screen = ShowScreen<ShopScreen>();
            screen.Setup(categoryType);
        }
    }
}