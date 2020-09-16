using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyCardPresenter : ILobbyCardPresenter
    {
        private static readonly StringNumbersCache stringNumbers = new StringNumbersCache(0, 100);

        private readonly LobbyRosterCard card;
        private readonly int index;
        private readonly ILobbyRosterItem item;
        private readonly IconsFactory itemsFactory;

        public int Index => index;

        public ILobbyRosterItem Item => item;

        public LobbyRosterCard Card => card;

        public LobbyCardPresenter(
            LobbyRosterCard card,
            int index,
            ILobbyRosterItem item,
            IconsFactory itemsFactory)
        {
            this.card = card;
            this.index = index;
            this.item = item;
            this.itemsFactory = itemsFactory;

            Setup();
        }

        public void Dispose()
        {
        }

        public void SetSelected(bool selected)
        {
            card.Outline.enabled = selected;
            card.transform.localScale = Vector3.one * (selected ? 1.1f : 1f);
        }

        public void Setup()
        {
            card.CardNameLabel.SetLocalizedText(item.ItemName);
            card.CardPowerLabelValue.text = item.Power.ToString().ToUpper();
            card.CardLevelLabel.text = item.Level.ToString().ToUpper();

            if (item is IBattleTrooperItem battleTrooperItem)
            {
                card.CardPowerPlus.SetActive(true);
                card.CardAdditionalPowerLabel.text = battleTrooperItem.ItemsTotalPower.ToString();
            }
            else
            {
                card.CardPowerPlus.SetActive(false);
                card.CardAdditionalPowerLabel.gameObject.SetActive(false);
            }

            card.Stars.Show((int)item.Grade);
            card.CardBackground.Gradient = card.Colors.GetCardColor(item.Grade);

            card.PurchaseView.gameObject.SetActive(false);
            var product = item.Product;
            if (product != null)
            {
                card.PurchaseView.gameObject.SetActive(true);

                if (product.IsLocked)
                {
                    card.ItemIcon.material = card.GrayscaleMaterial;
                    Color color = card.ItemIcon.color;
                    card.ItemIcon.color = new Color(color.r, color.g, color.b, 0.8f);

                    card.PurchaseView.SetupLock(stringNumbers.GetString(product.PlayerLevelForUnlock));
                }
                else
                {
                    var realPrice = product is InAppPurchaseRosterProduct inapp
                        ? inapp.FormattedPrice
                        : string.Empty;
                    card.PurchaseView.SetupPrice(
                        product.Price,
                        realPrice);
                }
            }

            card.ItemIcon.sprite = itemsFactory.SpriteForItem(item);
        }

        public void UpdateWeaponIcon(WeaponName weaponName)
        {
            card.WeaponIcon.sprite = itemsFactory.SmallSpriteForWeaponItem(weaponName);
        }

        public void SetVisibleWeaponIcon(bool isVisible)
        {
            card.WeaponIcon.gameObject.SetActive(isVisible);
        }
    }
}