using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Lobby
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image background;

        [SerializeField]
        private Image foreground;

        [SerializeField]
        [Range(0, 1)]
        private float progress;

        public float Progress
        {
            get => progress;
            set
            {
                progress = Mathf.Clamp01(value);
                UpdateProgress();
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            foreground.rectTransform.offsetMax = new Vector2(
                -background.rectTransform.rect.width * (1 - progress),
                0);
        }
    }
}