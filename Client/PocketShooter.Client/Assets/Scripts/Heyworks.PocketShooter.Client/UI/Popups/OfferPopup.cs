using System;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class OfferPopup : BaseScreen
    {
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button spaceCloseButton;

        [SerializeField]
        private Button acceptButton;

        [SerializeField]
        private Text titleLabel;

        [SerializeField]
        private Text descriptionLabel;

        [SerializeField]
        private Text trooperNameLabel;

        [SerializeField]
        private Image banner;

        [SerializeField]
        private GameObject discountView;

        [SerializeField]
        private Text discountLabel;

        public Text TitleLabel => titleLabel;

        public Text DescriptionLabel => descriptionLabel;

        public Text TrooperNameLabel => trooperNameLabel;

        public Image Banner => banner;

        public GameObject DiscountView => discountView;

        public Text DiscountLabel => discountLabel;

        public event Action AcceptButtonOnClick;

        public event Action CloseButtonOnClick;

        private void OnEnable()
        {
            closeButton.onClick.AddListener(CloseButtonOnClicked);
            spaceCloseButton.onClick.AddListener(CloseButtonOnClicked);
            acceptButton.onClick.AddListener(AcceptButtonOnClicked);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(CloseButtonOnClicked);
            spaceCloseButton.onClick.RemoveListener(CloseButtonOnClicked);
            acceptButton.onClick.RemoveListener(AcceptButtonOnClicked);
        }

        private void CloseButtonOnClicked()
        {
            CloseButtonOnClick?.Invoke();
        }

        private void AcceptButtonOnClicked()
        {
            AcceptButtonOnClick?.Invoke();
        }
    }
}