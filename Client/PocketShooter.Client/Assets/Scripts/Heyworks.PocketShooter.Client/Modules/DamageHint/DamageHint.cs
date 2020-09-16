using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.Modules.DamageHints
{
    internal class DamageHint : MonoBehaviour
    {
        [SerializeField]
        private Text messageLabel;

        private Vector3 initialScale;

        private DamageHintSettings settings;

        private Coroutine showCoroutine;

        private Transform cachedTransform;

        public Text MessageLabel => messageLabel;

        public event Action<DamageHint> ShowFinished;

        private void Awake()
        {
            cachedTransform = GetComponent<Transform>();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Setup(string text, DamageHintSettings settings, DamageType damageType)
        {
            this.settings = settings;

            messageLabel.text = text;

            cachedTransform.localScale = settings.InitScale;

            if (damageType == DamageType.Extra)
            {
                cachedTransform.localScale *= settings.ExtraHintScaleFactor;
            }

            initialScale = cachedTransform.localScale;
        }

        public void Show(float delay)
        {
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }

            showCoroutine = StartCoroutine(ShowEnumerator(delay));
        }

        private IEnumerator ShowEnumerator(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            for (float t = 0; t <= settings.ShowingTime; t += Time.unscaledDeltaTime)
            {
                Color old = messageLabel.color;
                messageLabel.color = new Color(old.r, old.g, old.b, settings.FadeCurve.Evaluate(t / settings.ShowingTime));

                var scaleLerpValue = settings.ScaleCurve.Evaluate(t / settings.ShowingTime);
                cachedTransform.localScale = Vector3.Lerp(initialScale, initialScale * settings.UpScaleMultiplier, scaleLerpValue);

                yield return null;
            }

            HintShowingFinished();
        }

        private void HintShowingFinished()
        {
            Color old = messageLabel.color;
            messageLabel.color = new Color(old.r, old.g, old.b, 0f);
            cachedTransform.localScale = initialScale;

            ShowFinished?.Invoke(this);
        }
    }
}