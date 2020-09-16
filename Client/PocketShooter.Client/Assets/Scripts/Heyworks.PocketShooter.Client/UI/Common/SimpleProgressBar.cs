using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Common
{
    /// <summary>
    /// Represents simple progress bar control.
    /// </summary>
    public class SimpleProgressBar : MonoBehaviour
    {
        private float progress;

        [SerializeField]
        private Image foregroundImage;

        /// <summary>
        /// Gets or sets progress bar value.
        /// </summary>
        public float Progress
        {
            get
            {
                return progress;
            }

            set
            {
                progress = value;
                UpdateRepresentation(Progress);
            }
        }

        /// <summary>
        /// Sets progress bar color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetColor(Color color)
        {
            foregroundImage.color = color;
        }

        private void UpdateRepresentation(float fillAmount)
        {
            foregroundImage.rectTransform.localScale = new Vector3(Mathf.Clamp01(fillAmount), 1f, 1f);
        }
    }
}
