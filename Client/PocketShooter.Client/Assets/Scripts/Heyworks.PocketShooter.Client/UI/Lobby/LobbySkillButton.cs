using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbySkillButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Image icon;

        public Image Icon => icon;

        public event Action OnClick;

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
            OnClick?.Invoke();
        }
    }
}