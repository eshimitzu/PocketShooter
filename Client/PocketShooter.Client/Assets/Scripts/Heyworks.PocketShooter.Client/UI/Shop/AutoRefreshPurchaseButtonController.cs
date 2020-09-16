using System;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class AutoRefreshPurchaseButtonController : MonoBehaviour
    {
        [SerializeField]
        private ShopPurchaseButton shopPurchaseButton;

        private const float RefreshPeriod = 0.5f;

        private Func<Price> priceProvider;
        private string overridePriceText;
        private string caption;
        private bool affordable;
        private bool useShadow;
        private float discount;

        private SimpleScheduler scheduler;

        public Button.ButtonClickedEvent OnClick => shopPurchaseButton.OnClick;

        private void Awake()
        {
            scheduler = new SimpleScheduler(RefreshViews, RefreshPeriod);
        }

        public void HideButton()
        {
            shopPurchaseButton.gameObject.SetActive(false);
        }

        public void ShowButton()
        {
            shopPurchaseButton.gameObject.SetActive(true);
        }

        public void Setup(
            Func<Price> priceProvider,
            string caption = null,
            bool affordable = true,
            string overridePriceText = null,
            bool useShadow = true,
            float discount = 0)
        {
            this.priceProvider = priceProvider;
            this.overridePriceText = overridePriceText;
            this.caption = caption;
            this.affordable = affordable;
            this.useShadow = useShadow;
            this.discount = discount;

            shopPurchaseButton.Setup(priceProvider(), caption, affordable, overridePriceText, useShadow, discount);
        }

        private void Update()
        {
            if (priceProvider == null)
            {
                return;
            }

            scheduler.TryExecute();
        }

        private void RefreshViews()
        {
            shopPurchaseButton.Setup(priceProvider(), caption, affordable, overridePriceText, useShadow, discount);
        }
    }
}
