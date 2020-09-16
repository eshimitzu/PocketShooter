using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.UI.Common;
using Heyworks.PocketShooter.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyRosterCard : LobbyRosterEmptyCard
    {
        [SerializeField]
        private Button fullcardButton;

        [SerializeField]
        private LobbyColorsPreset colors;

        [SerializeField]
        private Text cardNameLabel;

        [SerializeField]
        private Text cardPowerLabel;

        [SerializeField]
        private Text cardPowerLabelValue;

        [SerializeField]
        private GameObject cardPowerPlus;

        [SerializeField]
        private Text cardAdditionalPowerLabel;

        [SerializeField]
        private Text cardLevelLabel;

        [SerializeField]
        private AdvancedImage cardBackground;

        [SerializeField]
        private AdvancedOutline outline;

        [SerializeField]
        private StarsControl stars;

        [SerializeField]
        private RosterPurchaseView purchaseView;

        [SerializeField]
        private RectTransform itemRoot;

        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private Image weaponIcon;

        [SerializeField]
        private Material grayscaleMaterial;

        // AutoProperty
        public RectTransform ItemRoot => itemRoot;

        public AdvancedOutline Outline => outline;

        public Text CardNameLabel => cardNameLabel;

        public Text CardPowerLabelValue => cardPowerLabelValue;

        public Text CardAdditionalPowerLabel => cardAdditionalPowerLabel;

        public Text CardLevelLabel => cardLevelLabel;

        public GameObject CardPowerPlus => cardPowerPlus;

        public AdvancedImage CardBackground => cardBackground;

        public LobbyColorsPreset Colors => colors;

        public StarsControl Stars => stars;

        public RosterPurchaseView PurchaseView => purchaseView;

        public Image ItemIcon => itemIcon;

        public Image WeaponIcon => weaponIcon;

        public Material GrayscaleMaterial => grayscaleMaterial;

        public event Action OnCardClicked;

        // Functions

        private void Start()
        {
            cardPowerLabel.text = cardPowerLabel.text.ToUpper();
        }

        private void OnEnable()
        {
            fullcardButton.onClick.AddListener(CardOnClick);
        }

        private void OnDisable()
        {
            fullcardButton.onClick.RemoveListener(CardOnClick);
        }

        private void CardOnClick()
        {
            OnCardClicked?.Invoke();
        }
    }
}