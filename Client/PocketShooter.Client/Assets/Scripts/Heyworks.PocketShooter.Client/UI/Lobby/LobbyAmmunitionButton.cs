using System;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyAmmunitionButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Image icon;

        [SerializeField]
        private Image status;

        [SerializeField]
        private Text timer;

        [SerializeField]
        private Sprite upgradeSprite;

        [SerializeField]
        private Sprite timerSprite;

        [SerializeField]
        private UpgradeState state;

        [SerializeField]
        private Text powerLabelText;

        [SerializeField]
        private Text powerText;

        [SerializeField]
        private Image plusIcon;

        [SerializeField]
        private Image plusBg;

        public event Action OnClick;

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonOnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ButtonOnClick);
        }

        private void Start()
        {
            switch (state)
            {
                case UpgradeState.None:
                    timer.gameObject.SetActive(false);
                    status.gameObject.SetActive(false);
                    break;
                case UpgradeState.Available:
                    timer.gameObject.SetActive(false);
                    status.gameObject.SetActive(true);
                    status.sprite = upgradeSprite;
                    break;
                case UpgradeState.Cooldown:
                    icon.color = Color.gray;
                    timer.gameObject.SetActive(true);
                    status.gameObject.SetActive(true);
                    status.sprite = timerSprite;
                    break;
            }
        }

        public void Setup(Sprite sprite, string power, bool showPlus)
        {
            icon.sprite = sprite;
            powerLabelText.SetLocalizedText(LocKeys.PowerShortest);
            powerText.text = power;
            plusIcon.gameObject.SetActive(showPlus);
            plusBg.gameObject.SetActive(showPlus);
        }

        private void ButtonOnClick()
        {
            OnClick?.Invoke();
        }

        private void Setup()
        {
        }

        private enum UpgradeState
        {
            None = 0,
            Available = 1,
            Cooldown = 2,
        }
    }
}