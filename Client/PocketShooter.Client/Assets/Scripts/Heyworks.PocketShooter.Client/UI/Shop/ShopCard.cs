using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.UI.Common;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Lobby;
using Heyworks.PocketShooter.UI.Popups;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ShopCard : MonoBehaviour
    {
        [SerializeField]
        private AdvancedImage background;

        [SerializeField]
        private Button infoButton;

        [SerializeField]
        private RectTransform timerView;

        [SerializeField]
        private Text timerLabel;

        [SerializeField]
        private Text offerTitle;

        [SerializeField]
        private Image offerIcon;

        [SerializeField]
        private ShopRewardCurrencyCell currencyReward;

        [SerializeField]
        private ShopRewardItemCell itemReward;

        [SerializeField]
        private ShopPurchaseButton purchaseButton;

        [SerializeField]
        private GameObject discountBadge;

        [SerializeField]
        private Text discountLabel;

        [SerializeField]
        private List<ShopIcon> icons;

        private ShopProduct product;
        private Player player;

        public void Setup(ShopProduct product, Player player)
        {
            this.product = product;
            this.player = player;

            // Hide in the 0.2 version.
            timerView.gameObject.SetActive(false);
            discountBadge.SetActive(false);
            itemReward.gameObject.SetActive(false);
            currencyReward.gameObject.SetActive(false);
            infoButton.gameObject.SetActive(false);

            var settings = icons.SingleOrDefault(i => i.Name == product.Id);
            if (settings == null)
            {
                settings = icons.First();
                PurchaseLog.Log.LogWarning($"Can't find shop card settings for product with id {product.Id}. Using default one.");
            }

            background.Gradient = settings.Gradient.value;
            offerIcon.sprite = settings.Sprite;

            offerTitle.text = product.Name;

            var realPrice = product is InAppPurchaseShopProduct inapp
                ? inapp.FormattedPrice
                : string.Empty;
            purchaseButton.Setup(
                product.Price,
                string.Empty,
                player.CanPayPrice(product.Price),
                realPrice,
                false);
            purchaseButton.OnClick.AddListener(AcquireProduct);
        }

        private void OnEnable()
        {
            infoButton.onClick.AddListener(InfoOnClick);
        }

        private void OnDisable()
        {
            infoButton.onClick.RemoveListener(InfoOnClick);
        }

        private void InfoOnClick()
        {
            throw new NotImplementedException();
        }

        private void AcquireProduct()
        {
            IProductAcquiring productAcquiring = product;
            if (player.CanPayPrice(product.Price))
            {
                productAcquiring.AcquireFailed += ProductAcquiring_AcquireFailed;
                productAcquiring.AcquireSucceeded += ProductAcquiring_AcquireSucceeded;
                productAcquiring.Acquire();
            }
            else
            {
                var screen = ScreensController.Instance.ShowScreen<ShopScreen>();

                Price price = product.Price;
                if (price.GoldAmount > 0)
                {
                    screen.Setup(ShopCategory.Hard);
                }
                else if (price.CashAmount > 0)
                {
                    screen.Setup(ShopCategory.Soft);
                }
            }
        }

        private void ProductAcquiring_AcquireFailed(IProductAcquiring productAcquiring)
        {
            // TODO: a.dezhurko Handle Failed
            productAcquiring.AcquireFailed -= ProductAcquiring_AcquireFailed;
            productAcquiring.AcquireSucceeded -= ProductAcquiring_AcquireSucceeded;
        }

        private void ProductAcquiring_AcquireSucceeded(IProductAcquiring productAcquiring)
        {
            productAcquiring.AcquireFailed -= ProductAcquiring_AcquireFailed;
            productAcquiring.AcquireSucceeded -= ProductAcquiring_AcquireSucceeded;

            var screen = ScreensController.Instance.ShowPopup<AcquireConfirmationPopup>();
            screen.Setup(product.Content);
        }

        [Serializable]
        private class ShopIcon
        {
            public string Name;
            public Sprite Sprite;
            public GradientObject Gradient;
        }
    }
}