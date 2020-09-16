using UnityEngine;

namespace Heyworks.PocketShooter.UI.Animation
{
    [AddComponentMenu("HEYWORKS/Tween/Scale")]
    public class TweenScale : Tweener
    {
        #region Variables

        private Transform targetTransform;

        [SerializeField]
        private Vector3 endScale = Vector3.one;

        [SerializeField]
        private Vector3 beginScale = Vector3.zero;

        public Vector3 EndScale
        {
            get { return endScale; }
            set { endScale = value; }
        }

        public Vector3 BeginScale
        {
            get { return beginScale; }
            set { beginScale = value; }
        }

        public Transform TargetTransform
        {
            get
            {
                if (targetTransform == null)
                {
                    targetTransform = transform;
                }

                return targetTransform;
            }
        }

        public Vector3 CurrentScale
        {
            get { return TargetTransform.localScale; }
            set { TargetTransform.localScale = value; }
        }

        #endregion

        #region Private

        protected override void TweenUpdateRuntime(float factor, bool isFinished)
        {
            CurrentScale = BeginScale + (EndScale - BeginScale) * factor;
        }

        protected override void TweenUpdateEditor(float factor)
        {
            CurrentScale = BeginScale + (EndScale - BeginScale) * factor;
        }

        public static TweenScale SetScale(GameObject go, Vector3 scale, float duration = 1f)
        {
            var tws = InitGo<TweenScale>(go);
            tws.BeginScale = tws.CurrentScale;
            tws.EndScale = scale;
            tws.Duration = duration;
            tws.Play(true);
            return tws;
        }

        #endregion
    }
}