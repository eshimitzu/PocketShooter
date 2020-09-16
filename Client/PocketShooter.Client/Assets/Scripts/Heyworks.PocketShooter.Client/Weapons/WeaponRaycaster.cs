using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public abstract class WeaponRaycaster : MonoBehaviour
    {
        [SerializeField]
        private Vector3 aimViewportPoint = new Vector3(0.5f, 0.5f, 0f);

        [SerializeField]
        private LayerMask raycastLayerMask;

        [SerializeField]
        private LayerMask allyLayerMask;

        [SerializeField]
        private LayerMask enemyLayerMask;

        protected LayerMask targetLayerMask;

        public Transform ShotOriginTransfrom { get; private set; }

        protected UnityEngine.Camera MainCamera { get; private set; }

        protected Vector3 AimViewportPoint => aimViewportPoint;

        protected LayerMask RaycastLayerMask => raycastLayerMask;

        protected virtual void Awake() => MainCamera = UnityEngine.Camera.main;

        protected virtual void Start()
        {
            LayerMask layerMask = 1 << gameObject.layer;
            bool ally = (allyLayerMask & layerMask) > 0;
            targetLayerMask = ally ? enemyLayerMask : allyLayerMask;
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Setup(Transform shotOriginTransform)
        {
            ShotOriginTransfrom = shotOriginTransform;
        }

        protected bool IsTarget(int layer)
        {
            var layerMask = 1 << layer;
            return (layerMask & targetLayerMask) > 0;
        }
    }
}