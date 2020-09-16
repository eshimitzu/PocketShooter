using System;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class SettingsToggle : MonoBehaviour
    {
        [SerializeField]
        private Image background;

        [SerializeField]
        private Image toggle;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Text text;

        [SerializeField]
        private Color activeTextColor;

        [SerializeField]
        private Color inactiveTextColor;

        [SerializeField]
        private Color activeBackgroundColor;

        [SerializeField]
        private Color inactiveBackgroundColor;

        [SerializeField]
        private Color activeToggleColor;

        [SerializeField]
        private Color inactiveToggleColor;

        public event Action<bool> OnToggle;

        private bool toggleOn;

        public bool ToggleOn
        {
            get => toggleOn;
            set => SetToggle(value);
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonOnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ButtonOnClick);
        }

        private void SetToggle(bool on)
        {
            toggleOn = on;
            toggle.transform.localScale = new Vector3(toggleOn ? 1 : -1, 1, 1);
            text.color = toggleOn ? activeTextColor : inactiveTextColor;
            background.color = toggleOn ? activeBackgroundColor : inactiveBackgroundColor;
            toggle.color = toggleOn ? activeToggleColor : inactiveToggleColor;
            text.SetLocalizedText(toggleOn ? LocKeys.SettingsOn : LocKeys.SettingsOff);
        }

        private void ButtonOnClick()
        {
            ToggleOn = !ToggleOn;
            OnToggle?.Invoke(ToggleOn);
        }
    }
}