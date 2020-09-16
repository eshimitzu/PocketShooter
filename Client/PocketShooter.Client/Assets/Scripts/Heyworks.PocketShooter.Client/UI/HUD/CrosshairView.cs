using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    public class CrosshairView : MonoBehaviour
    {
        [SerializeField]
        private Image crosshairImage;

        [SerializeField]
        private Image warmupProgressBar;

        private bool isSniperScope;

        public float WarmupProgress
        {
            get => warmupProgressBar.fillAmount;
            set => warmupProgressBar.fillAmount = value;
        }

        public void SetCrosshair(Sprite crosshair, float size)
        {
            crosshairImage.enabled = crosshair != null;
            crosshairImage.sprite = crosshair;
            crosshairImage.rectTransform.sizeDelta = new Vector2(size, size);
        }

        public void SetVisible(bool show)
        {
            crosshairImage.enabled = !isSniperScope && show && (crosshairImage.sprite != null);
        }

        public void SetSniperScope(bool isSniperScope)
        {
            this.isSniperScope = isSniperScope;
            SetVisible(!isSniperScope);
        }
    }
}