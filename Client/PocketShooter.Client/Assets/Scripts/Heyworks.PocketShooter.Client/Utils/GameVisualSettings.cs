using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Represents game color settings.
    /// </summary>
    /// <seealso cref="UnityEngine.ScriptableObject" />
    [CreateAssetMenu(fileName = "GameVisualSettings", menuName = "Heyworks/Settings/Create Game Visual Settings")]
    public class GameVisualSettings : ScriptableObject
    {
        [SerializeField]
        private DamageHintColors damageHint = null;

        [SerializeField]
        private BattleProgressBarColors battleProgressBar = null;

        [SerializeField]
        private Color friendColor = default(Color);
        [SerializeField]
        private Color enemyColor = default(Color);
        [SerializeField]
        private Color neutralColor = default(Color);
        [SerializeField]
        private Color playerColor = default(Color);
        [SerializeField]
        private Color friendCaptureZoneColor = default(Color);
        [SerializeField]
        private Color enemyCaptureZoneColor = default(Color);
        [SerializeField]
        private int healthForSegment;
        [SerializeField]
        private int armorForSegment;
        [SerializeField]
        private int maxKillSeries;
        [SerializeField]
        private float killSeriesTimeOut;
        [SerializeField]
        private float notificationfadeSpeed;
        [SerializeField]
        private int notificationTimeToFinishMatch;

        /// <summary>
        /// Gets the damage hint.
        /// </summary>
        public DamageHintColors DamageHint => damageHint;

        public Color FriendColor => friendColor;

        public Color EnemyColor => enemyColor;

        public Color NeutralColor => neutralColor;

        public Color PlayerColor => playerColor;

        public Color FriendCaptureZoneColor => friendCaptureZoneColor;

        public Color EnemyCaptureZoneColor => enemyCaptureZoneColor;

        public int HealthForSegment => healthForSegment;

        public int ArmorForSegment => armorForSegment;

        public int MaxKillSeries => maxKillSeries;

        public float KillSeriesTimeOut => killSeriesTimeOut;

        public float NotificationFadeSpeed => notificationfadeSpeed;

        public int NotificationTimeToFinishMatch => notificationTimeToFinishMatch;

        /// <summary>
        /// Gets the battle progress bar.
        /// </summary>
        public BattleProgressBarColors BattleProgressBar => battleProgressBar;

        /// <summary>
        /// Represents damage hint colors.
        /// </summary>
        [Serializable]
        public class DamageHintColors
        {
            [SerializeField]
            private Color defaultDamageColor = new Color32(255, 255, 255, 255);

            [SerializeField]
            private Color criticalDamageColor = new Color32(247, 23, 54, 255);

            /// <summary>
            /// Gets the default color of the damage.
            /// </summary>
            public Color DefaultDamageColor => defaultDamageColor;

            /// <summary>
            /// Gets the color of the critical damage.
            /// </summary>
            public Color CriticalDamageColor => criticalDamageColor;
        }

        /// <summary>
        /// Represents colors container for battle ui progressbars.
        /// </summary>
        [Serializable]
        public class BattleProgressBarColors
        {
            [SerializeField]
            private Color increaseColor = new Color32(255, 255, 255, 255);

            [SerializeField]
            private Color decreaseColor = new Color32(247, 23, 54, 255);

            /// <summary>
            /// Gets the color of the increase.
            /// </summary>
            public Color IncreaseColor => increaseColor;

            /// <summary>
            /// Gets the color of the decrease.
            /// </summary>
            public Color DecreaseColor => decreaseColor;
        }
    }
}