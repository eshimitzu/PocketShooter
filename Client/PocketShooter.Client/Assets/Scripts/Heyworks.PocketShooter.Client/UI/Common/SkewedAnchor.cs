using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Heyworks.PocketShooter.UI.Common
{
    [ExecuteAlways]
    public class SkewedAnchor : UIBehaviour
    {
        [SerializeField]
        private RectTransform skewAnchor;

        [SerializeField]
        private float skewX = 0.065f;

        [SerializeField]
        private float skewY;

        private RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        protected override void Awake()
        {
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void SetDirty()
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(nameof(Build));
            }
        }

        private IEnumerator Build()
        {
            yield return new WaitForEndOfFrame();

            Vector2 center = Vector2.zero;
            RectTransform.anchoredPosition = Vector2.zero;

            if (skewAnchor)
            {
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, skewAnchor.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    RectTransform,
                    screenPosition,
                    null,
                    out Vector2 localPoint);
                center = localPoint;
            }

            if (skewX > 0 || skewY > 0)
            {
                Matrix4x4 skew = Matrix4x4.identity;
                skew[0, 1] = skewX;
                skew[1, 0] = skewY;

                Vector2 skewPoint = skew.MultiplyPoint(RectTransform.anchoredPosition - center);
                RectTransform.anchoredPosition = center + skewPoint;
            }
        }
    }
}