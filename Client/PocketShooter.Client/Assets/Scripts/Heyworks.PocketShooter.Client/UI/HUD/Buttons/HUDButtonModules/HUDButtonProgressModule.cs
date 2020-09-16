using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules
{
    internal class HUDButtonProgressModule : MonoBehaviour
    {
        [SerializeField]
        private Image progressBar;

        private bool isProgressBarActive;
        private float progressBarDuration;

        private int progressFinishAt;

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void StartProgress(int tickerCurrent, int duration)
        {
            progressBar.fillAmount = 1f;
            isProgressBarActive = true;

            progressBarDuration = duration;

            progressFinishAt = tickerCurrent + duration;

            UpdateProgress(tickerCurrent);
        }

        public void UpdateProgress(int tickerCurrent)
        {
            if (isProgressBarActive)
            {
                int remainingProgressTime = progressFinishAt - tickerCurrent;

                if (remainingProgressTime < 0)
                {
                    progressBar.fillAmount = 0f;

                    isProgressBarActive = false;
                }
                else
                {
                    progressBar.fillAmount = 1 - (remainingProgressTime / progressBarDuration);
                }
            }
        }
    }
}