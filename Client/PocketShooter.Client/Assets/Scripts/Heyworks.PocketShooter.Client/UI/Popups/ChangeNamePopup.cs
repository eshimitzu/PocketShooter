using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Utils;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class ChangeNamePopup : BaseScreen
    {
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button spaceCloseButton;

        [SerializeField]
        private Button changeNameButton;

        [SerializeField]
        private InputField nameField;

        [SerializeField]
        private Text errorText;

        private MetaGame game;

        public void Setup(MetaGame game)
        {
            this.game = game;

            errorText.gameObject.SetActive(false);
            changeNameButton.interactable = false;
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(CloseButtonOnClick);
            spaceCloseButton.onClick.AddListener(CloseButtonOnClick);
            nameField.onValueChanged.AddListener(NameFieldOnValueChanged);
            changeNameButton.onClick.AddListener(ChangeNameButtonOnClick);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(CloseButtonOnClick);
            spaceCloseButton.onClick.RemoveListener(CloseButtonOnClick);
            nameField.onValueChanged.RemoveListener(NameFieldOnValueChanged);
            changeNameButton.onClick.RemoveListener(ChangeNameButtonOnClick);
        }

        private void CloseButtonOnClick()
        {
            Hide();
        }

        private void NameFieldOnValueChanged(string value)
        {
            var isValid = NicknameValidator.IsValid(value);
            changeNameButton.interactable = isValid;
            errorText.gameObject.SetActive(!isValid && value != string.Empty);
        }

        private async void ChangeNameButtonOnClick()
        {
            var newNickname = nameField.text;
            var response = await game.GameHubClient.ChangeNickname(newNickname);

            if (response.IsOk)
            {
                game.Player.Nickname = newNickname;
                Hide();
            }
            else
            {
                errorText.gameObject.SetActive(true);
            }
        }
    }
}