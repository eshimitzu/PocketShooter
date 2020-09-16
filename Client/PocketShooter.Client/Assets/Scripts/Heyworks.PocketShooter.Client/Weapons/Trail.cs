using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    /// <summary>
    /// Represents trail vfx.
    /// </summary>
    public class Trail : MonoBehaviour
    {
        [SerializeField]
        private float trailSpeed = 10f;
        [SerializeField]
        private bool useFadeOut = true;
        [SerializeField]
        private float fadeOutTime = 2.0f;
        [SerializeField]
        private iTween.EaseType fadeOutEaseType = iTween.EaseType.linear;
        [SerializeField]
        private bool useScaleUp = true;
        [SerializeField]
        private float scaleTime = 2.0f;
        [SerializeField]
        private float scaleMultiplier = 3f;
        [SerializeField]
        private iTween.EaseType scaleEaseType = iTween.EaseType.linear;

        private TrailRenderer trailRenderer;
        private Coroutine coroutine;
        private Gradient initialGradient;
        private GradientAlphaKey[] initialAlphaKeys;
        private float initialStartWidth;
        private float initialEndWidth;

        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            initialGradient = trailRenderer.colorGradient;
            initialAlphaKeys = initialGradient.alphaKeys;
            initialStartWidth = trailRenderer.startWidth;
            initialEndWidth = trailRenderer.endWidth;
        }

        /// <summary>
        /// Move the trail.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Move(Vector3 startPoint, Vector3 endPoint)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            iTween.Stop(gameObject);

            transform.position = startPoint;
            trailRenderer.Clear();
            coroutine = StartCoroutine(MoveCoroutine(startPoint, endPoint));
        }

        /// <summary>
        /// Resets the trail.
        /// </summary>
        public void Reset()
        {
            trailRenderer.Clear();
            trailRenderer.colorGradient = initialGradient;
            trailRenderer.startWidth = initialStartWidth;
            trailRenderer.endWidth = initialEndWidth;
        }

        private IEnumerator MoveCoroutine(Vector3 startPoint, Vector3 endPoint)
        {
            yield return null;

            var progress = 0f;

            while (progress <= 1.0f)
            {
                progress += Time.deltaTime * trailSpeed;
                transform.position = Vector3.Lerp(startPoint, endPoint, progress);

                yield return null;
            }

            Lean.Pool.LeanPool.Despawn(gameObject, trailRenderer.time);

            if (useFadeOut)
            {
                FadeOutTrail();
            }

            if (useScaleUp)
            {
                ScaleTrail();
            }
        }

        private void FadeOutTrail()
        {
            var hash = iTween.Hash(
                iT.ValueTo.time,
                fadeOutTime,
                iT.ValueTo.from,
                1,
                iT.ValueTo.to,
                0,
                iT.ValueTo.easetype,
                fadeOutEaseType,
                iT.ValueTo.onupdate,
                nameof(ITween_OnAlphaTweenUpdate));

            iTween.ValueTo(gameObject, hash);
        }

        private void ScaleTrail()
        {
            var hash = iTween.Hash(
                iT.ValueTo.time,
                scaleTime,
                iT.ValueTo.from,
                1,
                iT.ValueTo.to,
                scaleMultiplier,
                iT.ValueTo.easetype,
                scaleEaseType,
                iT.ValueTo.onupdate,
                nameof(ITween_OnScaleTweenUpdate));

            iTween.ValueTo(gameObject, hash);
        }

        private void ITween_OnAlphaTweenUpdate(float value)
        {
            MultiplyTrailAlpha(value);
        }

        private void ITween_OnScaleTweenUpdate(float value)
        {
            ScaleTrail(value);
        }

        private void ScaleTrail(float value)
        {
            trailRenderer.startWidth = initialStartWidth * value;
            trailRenderer.endWidth = initialEndWidth * value;
        }

        private void MultiplyTrailAlpha(float value)
        {
            var gradient = trailRenderer.colorGradient;
            var alphaKeys = gradient.alphaKeys;

            for (int j = 0; j < alphaKeys.Length; j++)
            {
                alphaKeys[j].alpha = initialAlphaKeys[j].alpha * value;
            }

            gradient.alphaKeys = alphaKeys;
            trailRenderer.colorGradient = gradient;
        }
    }
}
