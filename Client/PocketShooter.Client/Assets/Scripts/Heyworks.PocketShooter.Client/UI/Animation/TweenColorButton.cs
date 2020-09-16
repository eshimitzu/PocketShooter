using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Animation
{
    public class TweenColorButton : MonoBehaviour, ITweenColor
    {
        private Button button;

        public Color Color
        {
            get
            {
                if (button == null)
                {
                    button = GetComponent<Button>();
                }

                return button.image.color;
            }

            set
            {
                if (button == null)
                {
                    button = GetComponent<Button>();
                }

                button.image.color = value;
            }
        }
    }
}