using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.PainHUD
{
    /// <summary>
    /// Represents a pain HUD view.
    /// </summary>
    public class PainHUDView : MonoBehaviour
    {
        [SerializeField]
        private Image damageIndicator;
        [SerializeField]
        private Image damageSplash;
        [SerializeField]
        private Image deathSplash;

        private List<Image> damageIndicators = new List<Image>();

        private IPainHUDStateProvider painHudStateProvider;

        /// <summary>
        /// Initializes the view with pain HUD state provider.
        /// </summary>
        /// <param name="painHudStateProvider">painHudStateProvider.</param>
        public void Init(IPainHUDStateProvider painHudStateProvider)
        {
            this.painHudStateProvider = painHudStateProvider;
        }

        private void LateUpdate()
        {
            var state = painHudStateProvider.UpdatePainHUDState();

            if (state.DamageSplashColorAlpha > 0)
            {
                damageSplash.enabled = true;
                var color = damageSplash.color;
                color.a = state.DamageSplashColorAlpha;
                damageSplash.color = color;
            }
            else
            {
                damageSplash.enabled = false;
            }

            deathSplash.enabled = state.DeathSplashColorEnabled;

            var oldSize = damageIndicators.Count;
            var newSize = state.Indicators.Count;
            if (oldSize > newSize)
            {
                for (int i = oldSize - 1; i > newSize - 1; i--)
                {
                    LeanPool.Despawn(damageIndicators[i]);
                    damageIndicators.RemoveAt(i);
                }
            }
            else if (oldSize < newSize)
            {
                for (int i = 0; i < newSize - oldSize; i++)
                {
                    damageIndicators.Add(LeanPool.Spawn(damageIndicator, Vector3.zero, Quaternion.identity, transform));
                }
            }

            for (int i = 0; i < newSize; i++)
            {
                var color = damageIndicators[i].color;
                color.a = state.Indicators[i].Alpha;
                damageIndicators[i].color = color;
                damageIndicators[i].transform.localScale = Vector3.one * state.Indicators[i].Scale;
                damageIndicators[i].transform.rotation = Quaternion.Euler(0, 0, state.Indicators[i].Angle);
            }
        }
    }
}