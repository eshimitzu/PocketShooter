using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Animation
{
    public class TweenColorText : MonoBehaviour, ITweenColor
    {
        private Text text;

        public Color Color
        {
            get
            {
                if (text == null)
                {
                    text = GetComponent<Text>();
                }

                return text.color;
            }

            set
            {
                if (text == null)
                {
                    text = GetComponent<Text>();
                }

                text.color = value;
            }
        }
    }
}