using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Heyworks.PocketShooter.UI
{
    public class MatchmakingScreen : BaseScreen
    {
        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Text tipsLabel;

        [SerializeField]
        private Text timerLabel;

        [SerializeField]
        private List<string> tips;

        [Inject]
        private RealtimeRunBehavior runBehavior;

        private TimeSpan elapsed;

        public void Setup()
        {
            AddSecond();
            tipsLabel.text = tips.RandomObject();
            SchedulerManager.Instance.AddSchedulerTask(new SchedulerTask(SchedulerManager.Instance, this, TimerUpdate, 1f, true));
        }

        private void OnEnable()
        {
            backButton.onClick.AddListener(BackOnClick);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(BackOnClick);
        }

        private void TimerUpdate()
        {
            AddSecond();
        }

        private void BackOnClick()
        {
            runBehavior.ManualDisconnect();
        }

        private void AddSecond()
        {
            elapsed = elapsed.Add(new TimeSpan(0, 0, 1));
            timerLabel.text = $"{elapsed.Seconds} {LocKeys.SecondsShort.Localized()}";
        }
    }
}