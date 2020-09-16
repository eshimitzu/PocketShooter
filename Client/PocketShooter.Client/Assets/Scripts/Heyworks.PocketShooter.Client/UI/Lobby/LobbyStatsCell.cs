using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyStatsCell : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        [SerializeField]
        private Text titleLabel;

        [SerializeField]
        private Text currentValueLabel;

        [SerializeField]
        private Image arrow;

        [SerializeField]
        private Text nextValueLabel;

        public void Setup(Sprite statIcon, string title, string currentValue, Color nextValueColor, string nextValue = null)
        {
            if (icon != null)
            {
                icon.sprite = statIcon;
            }

            titleLabel.SetLocalizedText(title);
            currentValueLabel.text = currentValue;

            if (!string.IsNullOrEmpty(nextValue))
            {
                arrow.enabled = true;
                nextValueLabel.enabled = true;
                nextValueLabel.text = nextValue;
                nextValueLabel.color = nextValueColor;
            }
            else
            {
                arrow.enabled = false;
                nextValueLabel.enabled = false;
            }
        }
    }
}