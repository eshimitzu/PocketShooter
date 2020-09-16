using System;
using Heyworks.PocketShooter.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class PopupOptionView : MonoBehaviour
    {
        [SerializeField]
        private Button selectionButton;

        [SerializeField]
        private Text optionDescriptionShorthand;

        [SerializeField]
        private AdvancedImage background;

        public event Action Selected;

        private Action onSelect;

        private void Awake()
        {
            selectionButton.onClick.AddListener(SelectionButton_Click);
        }

        public void Setup(PopupOptionInfo optionInfo, Gradient defaultColor)
        {
            if (optionInfo == null)
            {
                gameObject.SetActive(false);
                return;
            }

            onSelect = optionInfo.Action;
            optionDescriptionShorthand.text = optionInfo.Text;
            background.Gradient = optionInfo.ButtonColor ?? defaultColor;
        }

        private void SelectionButton_Click()
        {
            Selected?.Invoke();
            onSelect?.Invoke();
        }
    }
}