using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules
{
    internal class HUDButtonCooldownModule : MonoBehaviour
    {
        [SerializeField]
        private Text countDownLabel;

        private int coolDownTimeFinishAt;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void StartCountDownTimer(int tickerCurrent, int coolDownTime)
        {
            coolDownTimeFinishAt = tickerCurrent + coolDownTime;
            gameObject.SetActive(true);

            UpdateCountDownTimer(tickerCurrent);
        }

        public void UpdateCountDownTimer(int tickerCurrent)
        {
            if (gameObject.activeSelf)
            {
                int remainingCoolDownTime = coolDownTimeFinishAt - tickerCurrent;

                if (remainingCoolDownTime < 0)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    countDownLabel.text = ((int)Constants.ToSeconds(remainingCoolDownTime - 1) + 1).ToString("N0");
                }
            }
        }
    }
}