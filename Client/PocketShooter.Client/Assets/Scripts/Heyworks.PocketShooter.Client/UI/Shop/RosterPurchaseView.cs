using System.Globalization;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Common;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class RosterPurchaseView : MonoBehaviour
    {
        [SerializeField]
        private LobbyColorsPreset colorsPreset;

        [SerializeField]
        private AdvancedImage background;

        [SerializeField]
        private Text priceLabel;

        [SerializeField]
        private GameObject coinIconCell;

        [SerializeField]
        private GameObject bucksIconCell;

        [SerializeField]
        private GameObject lockIconCell;

        [SerializeField]
        private GameObject spacePriceCell;

        public void SetupPrice(Price price, string formattedRealPrice = "")
        {
            Gradient bgColor = colorsPreset.GetPriceColorForRoster(price);
            background.Gradient = bgColor;

            priceLabel.alignment = TextAnchor.MiddleCenter;

            CultureInfo ci = new CultureInfo("ru-RU");

            switch (price.Type)
            {
                case PriceType.Cash:
                    coinIconCell.gameObject.SetActive(false);
                    bucksIconCell.gameObject.SetActive(true);
                    spacePriceCell.gameObject.SetActive(true);
                    priceLabel.text = price.CashAmount.ToString("N0", ci);
                    break;
                case PriceType.Gold:
                    coinIconCell.gameObject.SetActive(true);
                    bucksIconCell.gameObject.SetActive(false);
                    spacePriceCell.gameObject.SetActive(true);
                    priceLabel.text = price.GoldAmount.ToString("N0", ci);
                    break;
                case PriceType.RealCurrency:
                    coinIconCell.gameObject.SetActive(false);
                    bucksIconCell.gameObject.SetActive(false);
                    spacePriceCell.gameObject.SetActive(false);
                    priceLabel.text = formattedRealPrice;
                    break;
            }

            lockIconCell.gameObject.SetActive(false);
        }

        public void SetupLock(string unlockLevel)
        {
            background.Gradient = colorsPreset.LockRosterGradient;

            lockIconCell.gameObject.SetActive(true);
            coinIconCell.gameObject.SetActive(false);
            bucksIconCell.gameObject.SetActive(false);
            spacePriceCell.gameObject.SetActive(true);

            priceLabel.text = (LocKeys.LevelShort.Localized() + " " + unlockLevel).ToUpper();
        }
    }
}