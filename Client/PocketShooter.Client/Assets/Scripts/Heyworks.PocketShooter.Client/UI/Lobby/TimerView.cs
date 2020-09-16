using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Lobby
{
    public class TimerView : MonoBehaviour
    {
        private const float RefreshPeriod = 0.5f;

        [SerializeField]
        private Text captionText;
        [SerializeField]
        private Image progressBar;
        [SerializeField]
        private Text timeLeftText;
        [SerializeField]
        private bool invertProgressBar;

        private ITimer timer;
        private SimpleScheduler scheduler;

        public void Setup(ITimer timer, string caption)
        {
            captionText.SetLocalizedText(caption);
            this.timer = timer;

            UpdateProgress();
        }

        private void Awake()
        {
            scheduler = new SimpleScheduler(UpdateProgress, RefreshPeriod);
        }

        private void Update()
        {
            if (timer == null || timer.IsFinished)
            {
                return;
            }

            scheduler.TryExecute();
        }

        private void UpdateProgress()
        {
            var progress = invertProgressBar ? 1 - timer.Progress : timer.Progress;
            progressBar.fillAmount = progress;
            timeLeftText.text = timer.RemainingTime.ToShortCoupleTimeComponentString();
        }
    }
}
