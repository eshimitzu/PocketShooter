using System.Collections;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.UI.HUD;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents remote character view.
    /// </summary>
    /// <seealso cref="CharacterView" />
    public class RemoteCharacterView : CharacterView
    {
        private const float StatusBarHidingTime = 3f;

        [SerializeField]
        private CharacterStatusBarView statusBarView = null;

        private Coroutine hideStatusBarCoroutine;
        private bool isEnemy;
        private EntityId modelId;
        private TrooperClass trooperClass;

        /// <summary>
        /// Setups the status bar.
        /// </summary>
        public void SetupStatusBar(
            IRemotePlayer player,
            bool isEnemy)
        {
            this.isEnemy = isEnemy;

            statusBarView.gameObject.SetActive(!isEnemy);
            statusBarView.Setup(player, isEnemy);
        }

        /// <summary>
        /// Shows the status bar.
        /// </summary>
        /// <param name="health">The health.</param>
        /// <param name="armor">The armor.</param>
        public void ShowStatusBar(IRemotePlayer player)
        {
            if (hideStatusBarCoroutine != null && isEnemy)
            {
                ResetHideStatusBarCoroutine();
            }

            statusBarView.gameObject.SetActive(true);
            statusBarView.UpdateStatusBar(player);

            if (isEnemy)
            {
                hideStatusBarCoroutine = StartCoroutine(HideStatusBar());
            }
        }

        /// <summary>
        /// Hide the status bar immmediately.
        /// </summary>
        public void HideStatusBarImmediately()
        {
            statusBarView.gameObject.SetActive(false);
        }

        private IEnumerator HideStatusBar()
        {
            yield return new WaitForSeconds(StatusBarHidingTime);

            statusBarView.gameObject.SetActive(false);
        }

        private void ResetHideStatusBarCoroutine()
        {
            StopCoroutine(hideStatusBarCoroutine);
            hideStatusBarCoroutine = null;
        }
    }
}
