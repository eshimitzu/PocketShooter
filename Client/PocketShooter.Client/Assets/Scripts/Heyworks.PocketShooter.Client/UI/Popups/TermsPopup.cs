using System;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class TermsPopup : BaseScreen
    {
        [SerializeField]
        private Button okButton;

        [SerializeField]
        private Button termsButton;

        private const string privacyURL = "https://privacy.azurgames.com/";

        public event Action Successed;

        private void OnEnable()
        {
            okButton.onClick.AddListener(OkButtonOnClick);
            termsButton.onClick.AddListener(TermsButtonButtonOnClick);
        }

        private void OnDisable()
        {
            okButton.onClick.RemoveListener(OkButtonOnClick);
            termsButton.onClick.RemoveListener(TermsButtonButtonOnClick);
        }

        private void OkButtonOnClick()
        {
            Successed?.Invoke();
            Hide();
        }

        private void TermsButtonButtonOnClick()
        {
            Application.OpenURL(privacyURL);
        }
    }
}