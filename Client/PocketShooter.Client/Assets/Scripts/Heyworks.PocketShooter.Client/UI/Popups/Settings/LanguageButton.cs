using System;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class LanguageButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Text label;

        [SerializeField]
        private Image background;

        [SerializeField]
        private Color activeTextColor;

        [SerializeField]
        private Color inactiveTextColor;

        [SerializeField]
        private Color activeBackgroundColor;

        [SerializeField]
        private Color inactiveBackgroundColor;

        private string language;

        public string GlobalLanguage { get; private set; }

        public string Language
        {
            get => language;
            set
            {
                GlobalLanguage = LocKeys.Languages.GetSupportedLanguageByKey(value);
                language = value;
                label.SetLocalizedText(language);
            }
        }

        public event Action<string> OnClick;

        public void SetActive(bool active)
        {
            background.color = active ? activeBackgroundColor : inactiveBackgroundColor;
            label.color = active ? activeTextColor : inactiveTextColor;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonOnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ButtonOnClick);
        }

        private void ButtonOnClick()
        {
            OnClick?.Invoke(GlobalLanguage);
        }
    }
}