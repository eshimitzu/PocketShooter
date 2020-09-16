using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class MatchTimerView : MonoBehaviour
    {
        [SerializeField]
        private Text minutesLeftLabel;
        [SerializeField]
        private Text secondsLeftLabel;

        private StringNumbersCache secondsCache = new StringNumbersCache(0, 60, true);
        private StringNumbersCache minutesCache = new StringNumbersCache(0, 20, false);

        private int timerFinishAt;

        private float totalSecondsLeft = -1f;

        private float TotalSecondsLeft
        {
            get
            {
                return totalSecondsLeft;
            }

            set
            {
                totalSecondsLeft = value;

                int minutesLeft = (int)TotalSecondsLeft / 60;
                int secondsLeft = (int)TotalSecondsLeft % 60;
                minutesLeftLabel.text = (minutesLeft < 10 ? "0" : string.Empty) + minutesCache.GetString(minutesLeft);
                secondsLeftLabel.text = secondsCache.GetString(secondsLeft);
            }
        }

        public void StartTimer(int tickerCurrent, int timerValue)
        {
            timerFinishAt = tickerCurrent + timerValue;

            UpdateTimer(tickerCurrent);
        }

        public void UpdateTimer(int tickerCurrent)
        {
            int remainingTime = timerFinishAt - tickerCurrent;

            if (remainingTime >= 0)
            {
                TotalSecondsLeft = Constants.ToSeconds(remainingTime);
            }
            else
            {
                TotalSecondsLeft = 0f;
            }
        }

        public void StopTimer()
        {
            totalSecondsLeft = -1f;
        }
    }
}
