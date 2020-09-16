using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    internal class DominationZoneProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image zoneProgressBar;
        [SerializeField]
        private Image zoneProgressBarBack;
        [SerializeField]
        private Text zoneLabel;
        [SerializeField]
        private GameVisualSettings gameVisualSettings;

        private Color zoneProgressBarBackFriendColor;
        private Color zoneProgressBarBackEnemyColor;
        private Color zoneProgressBarBackDefaultColor;

        private IZone zone;
        private DominationModeInfo config;
        private TeamNo team;

        public Image ZoneProgressBar { get => zoneProgressBar; }

        public Image ZoneProgressBarBack { get => zoneProgressBarBack; }

        public Text ZoneLabel { get => zoneLabel; }

        /// <summary>
        /// Setup the specified index, zone, config and team.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="zone">Zone.</param>
        /// <param name="config">Config.</param>
        /// <param name="team">LocalPlayer Team.</param>
        public void Setup(int index, IZone zone, DominationModeInfo config, TeamNo team)
        {
            this.zone = zone;
            this.config = config;
            this.team = team;

            string pointLocKey = null;

            switch (index)
            {
                case 0:
                    pointLocKey = LocKeys.PointA;
                    break;
                case 1:
                    pointLocKey = LocKeys.PointB;
                    break;
                case 2:
                    pointLocKey = LocKeys.PointC;
                    break;
            }

            if (pointLocKey != null)
            {
                zoneLabel.SetLocalizedText(pointLocKey);
            }
        }

        private void Awake()
        {
            zoneProgressBarBackFriendColor = gameVisualSettings.FriendColor;
            zoneProgressBarBackEnemyColor = gameVisualSettings.EnemyColor;
            zoneProgressBarBackDefaultColor = zoneProgressBarBack.color;
            zoneProgressBarBackEnemyColor.a = zoneProgressBarBackDefaultColor.a;
            zoneProgressBarBackFriendColor.a = zoneProgressBarBackDefaultColor.a;
        }

        private void Update()
        {
            float progress = zone.State.Progress / config.TimeplayersToCapture;
            zoneProgressBar.fillAmount = Mathf.Lerp(zoneProgressBar.fillAmount, progress, Time.deltaTime * 10);

            if (zone.State.OwnerTeam != TeamNo.None)
            {
                if (zone.State.OwnerTeam != team)
                {
                    zoneProgressBar.color = zoneProgressBarBackEnemyColor;
                }
                else
                {
                    zoneProgressBar.color = zoneProgressBarBackFriendColor;
                }
            }
        }
    }
}