using System;
using Heyworks.PocketShooter.Meta.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.UI
{
    [CreateAssetMenu(fileName = "LobbyColorsPreset", menuName = "Heyworks/UI/Lobby Colors Preset")]
    public class LobbyColorsPreset : ScriptableObject
    {
        [SerializeField]
        private Gradient greenButton;

        [SerializeField]
        private Gradient blueButton;

        [Space(10)]
        [SerializeField]
        private Gradient commonCardColor;

        [SerializeField]
        private Gradient uncommonCardColor;

        [SerializeField]
        private Gradient rareCardColor;

        [SerializeField]
        private Gradient epicCardColor;

        [SerializeField]
        private Gradient legendCardColor;

        [Space(10)]
        [SerializeField]
        private Gradient purchaseButtonReal;

        [SerializeField]
        private Gradient purchaseButtonGold;

        [SerializeField]
        private Gradient purchaseButtonCash;

        [SerializeField]
        private Gradient equippedButton;

        [SerializeField]
        private Color realPriceColor;

        [SerializeField]
        private Color notRealPriceColor;

        [SerializeField]
        private Gradient goldRosterPriceColorGradient;

        [SerializeField]
        private Gradient cashRosterPriceColorGradient;

        [SerializeField]
        private Gradient realRosterPriceColorGradient;

        [SerializeField]
        private Color cantAffordPriceColor;

        [SerializeField]
        private Gradient lockRosterGradient;

        [Space(10)]
        [SerializeField]
        private Color starsActiveColor;

        [SerializeField]
        private Color starsInactiveColor;

        public Gradient GreenButton => greenButton;

        public Gradient BlueButton => blueButton;

        public Gradient GetCardColor(Grade grade)
        {
            switch (grade)
            {
                case Grade.Common:
                    return commonCardColor;
                case Grade.Uncommon:
                    return uncommonCardColor;
                case Grade.Rare:
                    return rareCardColor;
                case Grade.Epic:
                    return epicCardColor;
                case Grade.Legendary:
                    return legendCardColor;
            }

            return null;
        }

        public Color GetStarsColor(bool active) => active ? starsActiveColor : starsInactiveColor;

        public (Gradient gradient, Color textColor) GetPriceColor(Price price)
        {
            switch (price.Type)
            {
                case PriceType.Cash:
                    return (purchaseButtonCash, notRealPriceColor);
                case PriceType.Gold:
                    return (purchaseButtonGold, notRealPriceColor);
                case PriceType.RealCurrency:
                    return (purchaseButtonReal, realPriceColor);
                default:
                    Throw.Argument(nameof(price.Type));
                    return default;
            }
        }

        public Gradient GetPriceColorForRoster(Price price)
        {
            switch (price.Type)
            {
                case PriceType.Cash:
                    return cashRosterPriceColorGradient;
                case PriceType.Gold:
                    return goldRosterPriceColorGradient;
                case PriceType.RealCurrency:
                    return realRosterPriceColorGradient;
            }

            return new Gradient();
        }

        public Color CantAffordPriceColor => cantAffordPriceColor;

        public Gradient GetEquipButtonColor(bool isEquipped) => isEquipped ? equippedButton : purchaseButtonReal;

        public Gradient LockRosterGradient => lockRosterGradient;
    }
}