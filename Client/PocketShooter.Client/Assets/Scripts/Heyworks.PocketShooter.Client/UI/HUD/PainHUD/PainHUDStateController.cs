using System.Collections.Generic;
using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD.PainHUD
{
    /// <summary>
    /// Manages the state of the pain HUD, e.g. Indocators angle and look,
    /// pain and death splash intensity.
    /// </summary>
    public class PainHUDStateController : IPainHUDStateProvider
    {
        /// <summary>
        /// describes an object that recently inflicted damage to the
        /// player. used to track direction and fade out arrows.
        /// </summary>
        private struct Inflictor
        {
            public Transform Transform;
            public float DamageTime;

            public Inflictor(Transform transform, float damageTime)
            {
                Transform = transform;
                DamageTime = damageTime;
            }
        }

        private const float MaxIndicatorScale = 0.2f;
        private const float DamageSplashFadeSpeed = 2f;
        private const float PainIntecity = 1f;
        private const float IndicatorAngleOffset = 0f;
        private const float IndicatorFadeTime = 1.5f;
        private const float IndicatorShakeTime = 0.125f;
        private const float IndicatorScale = 2f;

        private PainHUDState state = new PainHUDState();
        private Transform player;

        private List<Inflictor> inflictors = new List<Inflictor>();

        private bool inflicted;

        /// <summary>
        /// Initializes a new instance of the <see cref="PainHUDStateController"/> class.
        /// </summary>
        /// <param name="player">Local player transform.</param>
        public PainHUDStateController(Transform player)
        {
            this.player = player;
        }

        /// <summary>
        /// Update and return current pain HUD state.
        /// </summary>
        public PainHUDState UpdatePainHUDState()
        {
            UpdateIndicators();
            UpdateDamageSplash();

            return state;
        }

        /// <summary>
        /// picks up an incoming damage and composes
        /// the data needed to draw the various effects.
        /// </summary>
        /// <param name="attackerTransform">attackerTransform.</param>
        /// <param name="damage">damage.</param>
        /// <param name="maxHealth">maxHealth.</param>
        public void Damaged(Transform attackerTransform, float damage, float maxHealth)
        {
            state.DamageSplashColorAlpha += damage / maxHealth * PainIntecity;

            if (attackerTransform == null)
            {
                return;
            }

            bool create = true;
            var size = inflictors.Count;
            for (int i = 0; i < size; i++)
            {
                if (inflictors[i].Transform == attackerTransform)
                {
                    inflictors[i] = new Inflictor(attackerTransform, Time.time);
                    create = false;
                }
            }

            if (create)
            {
                inflictors.Add(new Inflictor(attackerTransform, Time.time));
            }

            inflicted = true;
        }

        /// <summary>
        /// enables blood splatter effect on the screen while the player is dead.
        /// </summary>
        public void Dead()
        {
            state.DeathSplashColorEnabled = true;
        }

        /// <summary>
        /// clears pain fx when player comes back to life.
        /// </summary>
        public void Clear()
        {
            state.DeathSplashColorEnabled = false;
            state.DamageSplashColorAlpha = 0f;

            var size = inflictors.Count;
            for (int i = 0; i < size; i++)
            {
                inflictors[i] = new Inflictor(inflictors[i].Transform, 0f);
            }
        }

        private void UpdateDamageSplash()
        {
            if (state.DamageSplashColorAlpha < 0.01f)
            {
                state.DamageSplashColorAlpha = 0f;
                return;
            }

            state.DamageSplashColorAlpha = Mathf.Lerp(state.DamageSplashColorAlpha, 0f, Time.deltaTime * DamageSplashFadeSpeed);
        }

        private void UpdateIndicators()
        {
            if (!inflicted)
            {
                return;
            }

            inflicted = false;
            state.Indicators.Clear();

            var size = inflictors.Count;
            for (int i = 0; i < size; i++)
            {
                var inflictor = inflictors[i];

                if (inflictor.Transform == null
                    || !inflictor.Transform.gameObject.activeSelf)
                {
                    inflictors.Remove(inflictor);
                    size--;
                    continue;
                }

                var alpha = (IndicatorFadeTime - (Time.time - inflictor.DamageTime)) / IndicatorFadeTime;

                if (alpha < 0f)
                {
                    continue;
                }

                if (inflictor.Transform == player.transform)
                {
                    continue;
                }

                float angle = -Utils3D.LookAtAngleHorizontal(player.position, player.forward, inflictor.Transform.position)
                            + IndicatorAngleOffset;

                float scale = (IndicatorShakeTime - (Time.time - inflictor.DamageTime)) / IndicatorShakeTime;
                scale = IndicatorScale * (1 + Mathf.Lerp(0f, MaxIndicatorScale, scale));

                state.Indicators.Add(new PainHUDState.Indicator(angle, scale, alpha));

                inflicted = true;
            }
        }
    }
}