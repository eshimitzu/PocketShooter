using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    /// <summary>
    /// Represents DominationModeView.
    /// </summary>
    public class DominationModeView : MonoBehaviour
    {
        [SerializeField]
        private Text scoreFriendTeamLabel;
        [SerializeField]
        private Text scoreEnemyTeamLabel;
        [SerializeField]
        private Image scoreFriendTeamProgressBar;
        [SerializeField]
        private Image scoreEnemyTeamProgressBar;
        [SerializeField]
        private DominationZoneProgressBar zoneProgressBarPrefab;
        [SerializeField]
        private RectTransform zonesRoot;
        [SerializeField]
        private GameVisualSettings gameVisualSettings;
        [SerializeField]
        private DominationZonePointer zonePointerPrefab;

        private float friendTeamScoreLerpA = 0.5f;
        private float friendTeamScoreLerpB = 0.5f;
        private float enemyTeamScoreLerpA = 0.5f;
        private float enemyTeamScoreLerpB = 0.5f;

        private float deltaTime;

        private StringNumbersCache pointsCache;
        private DominationModeInfo modeInfo;

        private ITeam friendTeam;
        private ITeam enemyTeam;

        public DominationZonePointer ZonePointerPrefab => zonePointerPrefab;

        private void Update()
        {
            var currentDeltaTime = Time.deltaTime;

            scoreFriendTeamLabel.text = pointsCache.GetString(friendTeam.State.Score);
            scoreEnemyTeamLabel.text = pointsCache.GetString(enemyTeam.State.Score);

            CalculateLerping();
            var targetScale = Mathf.Lerp(friendTeamScoreLerpA, friendTeamScoreLerpB, currentDeltaTime / deltaTime);
            scoreFriendTeamProgressBar.transform.localScale = new Vector3(targetScale, 1f, 1f);
            targetScale = Mathf.Lerp(enemyTeamScoreLerpA, enemyTeamScoreLerpB, currentDeltaTime / deltaTime);
            scoreEnemyTeamProgressBar.transform.localScale = new Vector3(targetScale, 1f, 1f);
        }

        /// <summary>
        /// Setup the specified game and config.
        /// </summary>
        /// <param name="game">Game.</param>
        /// <param name="playerTeam">The player's team number.</param>
        /// <param name="modeInfo">The game mode info.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Setup(IClientGame game, TeamNo playerTeam, DominationModeInfo modeInfo)
        {
            AssertUtils.NotNull(game, nameof(IClientGame));
            AssertUtils.NotNull(modeInfo, nameof(DominationModeInfo));

            this.modeInfo = modeInfo;

            deltaTime = Constants.ToSeconds(modeInfo.CheckInterval);
            scoreFriendTeamProgressBar.color = gameVisualSettings.FriendColor;
            scoreEnemyTeamProgressBar.color = gameVisualSettings.EnemyColor;
            pointsCache = new StringNumbersCache(0, modeInfo.WinScore);

            (friendTeam, enemyTeam) = playerTeam == game.Team1.Id
                ? (game.Team1, game.Team2)
                : (game.Team2, game.Team1);
            scoreFriendTeamProgressBar.transform.localScale = new Vector3(0f, 1f, 1f);
            scoreEnemyTeamProgressBar.transform.localScale = new Vector3(0f, 1f, 1f);
            CalculateLerping();

            int index = 0;
            foreach (var zone in game.Zones.Values)
            {
                var bar = Instantiate(zoneProgressBarPrefab, zonesRoot);
                bar.Setup(index, zone, modeInfo, playerTeam);

                index++;
            }
        }

        private void CalculateLerping()
        {
            friendTeamScoreLerpA = scoreFriendTeamProgressBar.transform.localScale.x;
            enemyTeamScoreLerpA = scoreEnemyTeamProgressBar.transform.localScale.x;
            friendTeamScoreLerpB = (float)friendTeam.State.Score / modeInfo.WinScore;
            enemyTeamScoreLerpB = (float)enemyTeam.State.Score / modeInfo.WinScore;
        }
    }
}