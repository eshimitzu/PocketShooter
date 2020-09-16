using System;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.UI.Common;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ShopCategoryButton : MonoBehaviour
    {
        [SerializeField]
        private AdvancedImage background;

        [SerializeField]
        private Text label;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Color activeTextColor;

        [SerializeField]
        private Color inactiveTextColor;

        [SerializeField]
        private Gradient activeBgGradient;

        [SerializeField]
        private Gradient inactiveBgGradient;

        public event Action OnClick;

        public void Setup(ShopCategory category)
        {
            label.SetLocalizedText(LocKeys.GetShopCategoryKey(category));
        }

        public void SetActive(bool active)
        {
            label.color = active ? activeTextColor : inactiveTextColor;
            background.Gradient = active ? activeBgGradient : inactiveBgGradient;
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
            OnClick?.Invoke();
        }
    }
}