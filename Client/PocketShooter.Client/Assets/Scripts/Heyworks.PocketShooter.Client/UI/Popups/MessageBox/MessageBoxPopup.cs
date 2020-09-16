using System;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class MessageBoxPopup : BaseScreen
    {
        private readonly (Vector2 min, Vector2 max) optionsAllAnchors = (new Vector2(.1f, 0f), new Vector2(0.86f, 1f));
        private readonly (Vector2 min, Vector2 max) optionsSingleAnchors = (new Vector2(.35f, 0f), new Vector2(0.65f, 1f));

        [SerializeField]
        private LobbyColorsPreset colorsPreset;

        [SerializeField]
        private Text titleLabel;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Text textLabel;

        [SerializeField]
        private PopupOptionView option1View;

        [SerializeField]
        private PopupOptionView option2View;

        [SerializeField]
        private RectTransform optionsTransform;

        private Action onCancel;

        private void Start()
        {
            option1View.Selected += OptionView_Selected;
            option2View.Selected += OptionView_Selected;
        }

        public void Setup(string title, string text, PopupOptionInfo option1, PopupOptionInfo option2 = null, Action onCancel = null)
        {
            titleLabel.text = title;
            closeButton.onClick.AddListener(CloseButton_Click);
            this.onCancel = onCancel ?? option1.Action;
            textLabel.text = text;

            option1View.Setup(option1, colorsPreset.GreenButton);
            option2View.Setup(option2, colorsPreset.BlueButton);
            optionsTransform.anchorMin = option2 != null ? optionsAllAnchors.min : optionsSingleAnchors.min;
            optionsTransform.anchorMax = option2 != null ? optionsAllAnchors.max : optionsSingleAnchors.max;
        }

        private void CloseButton_Click()
        {
            onCancel?.Invoke();

            Hide();
        }

        private void OptionView_Selected()
        {
            Hide();
        }
    }
}