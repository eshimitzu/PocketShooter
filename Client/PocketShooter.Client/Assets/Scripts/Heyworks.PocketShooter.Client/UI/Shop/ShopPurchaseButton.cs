using System;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Common;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ShopPurchaseButton : MonoBehaviour
    {
        // TODO: v.shimkovich need to have bigger bg for ingame price , because its can be 6 digit length (big). find better solution.
        private static int SmallBackgroundWidth = 130;
        private static int BigBackgroundWidth = 190;

        [SerializeField]
        private LobbyColorsPreset colorsPreset;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Text priceLabel;

        [SerializeField]
        private Text oldPriceLabel;
        [SerializeField]
        private GameObject oldPriceRoot;

        [SerializeField]
        private Text captionLabel;
        [SerializeField]
        private GameObject captionLabelRoot;

        [SerializeField]
        private AdvancedImage background;
        [SerializeField]
        private LayoutElement priceBackgroundElement;

        [SerializeField]
        private GameObject addIcon;
        [SerializeField]
        private GameObject coinIcon;
        [SerializeField]
        private GameObject bucksIcon;

        [SerializeField]
        private AdvancedShadow shadow;

        [SerializeField]
        private Text durationLabel;
        [SerializeField]
        private GameObject durationLabelRoot;

        public Button.ButtonClickedEvent OnClick => button.onClick;

        public void Setup(
            Price price,
            string caption = null,
            bool affordable = true,
            string overridePriceText = null,
            bool useShadow = true,
            float discount = 0,
            TimeSpan? actionDuration = null)
        {
            (Gradient bgColor, Color textColor) = colorsPreset.GetPriceColor(price);
            background.Gradient = bgColor;
            captionLabel.color = textColor;

            shadow.enabled = useShadow;

            addIcon.SetActive(false);
            coinIcon.SetActive(false);
            bucksIcon.SetActive(false);

            switch (price.Type)
            {
                case PriceType.Cash:
                    bucksIcon.SetActive(true);
                    priceBackgroundElement.preferredWidth = BigBackgroundWidth;
                    priceLabel.text = $"{price.CashAmount}";
                    break;
                case PriceType.Gold:
                    coinIcon.SetActive(true);
                    priceBackgroundElement.preferredWidth = BigBackgroundWidth;
                    priceLabel.text = $"{price.GoldAmount}";
                    break;
                case PriceType.RealCurrency:
                    priceBackgroundElement.preferredWidth = SmallBackgroundWidth;
                    priceLabel.text = $"{price.PurchaseId}";
                    break;
                case PriceType.AdTicket:
                    addIcon.SetActive(true);
                    priceBackgroundElement.preferredWidth = BigBackgroundWidth;
                    priceLabel.text = $"{price.TicketsAmount}";
                    break;
            }

            if (!string.IsNullOrEmpty(overridePriceText))
            {
                priceLabel.text = overridePriceText;
            }

            priceLabel.color = affordable ? textColor : colorsPreset.CantAffordPriceColor;
            priceLabel.alignment = TextAnchor.MiddleCenter;

            captionLabelRoot.SetActive(!string.IsNullOrEmpty(caption));
            captionLabel.SetLocalizedText(caption);

            SetupDiscount(discount);
            SetupDurationLabel(actionDuration);
        }

        private void SetupDurationLabel(TimeSpan? actionDuration)
        {

            if (!actionDuration.HasValue)
            {
                durationLabelRoot.SetActive(false);
            }
            else
            {
                durationLabelRoot.SetActive(true);
                durationLabel.text = actionDuration.Value.Equals(TimeSpan.Zero)
                    ? LocKeys.ItemInfo.InstantDuration.Localized()
                    : actionDuration.Value.ToShortCoupleTimeComponentString();
            }
        }

        private void SetupDiscount(float discount)
        {
            bool hasDiscount = discount > 0;
            oldPriceLabel.gameObject.SetActive(hasDiscount);

            // TODO: Handle discounts in proper way an in proper place when needed (move to lower level).
            // if (hasDiscount)
            // {
            //     float discountFactor = (100 - discount) / 100f;
            //     float priceValue = ((int)(realPrice / discountFactor * 100)) / 100f;
            //     oldPriceLabel.text = $"{priceValue}";
            //     priceLabel.alignment = TextAnchor.MiddleLeft;
            // }
        }
    }
}