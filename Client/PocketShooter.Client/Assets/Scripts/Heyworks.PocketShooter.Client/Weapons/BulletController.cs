using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    /// <summary>
    /// Represents controller for displaying trails.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BulletController : MonoBehaviour, IAttackVisualizer
    {
        [SerializeField]
        private Transform trailStartPoint = null;

        [SerializeField]
        private Trail trail = null;

        [SerializeField]
        private Trace trace = null;

        [SerializeField]
        private Vector3 traceSize = Vector3.one;

        [SerializeField]
        private HitImpactCollection impacts = null;

        private BulletImpactController impactController;

        public Transform TrailStartPoint => trailStartPoint;

        private void Awake()
        {
            impactController = new BulletImpactController(impacts);
        }

        /// <summary>
        /// Launches a bullet toward specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="onHit">The damage handler.</param>
        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        public void Attack(Vector3 point, Action<Vector3> onHit)
        {
            Vector3 position = trailStartPoint.position;
            if (trail)
            {
                var trailInstance = Lean.Pool.LeanPool.Spawn<Trail>(trail);
                trailInstance.Reset();
                trailInstance.Move(position, point);
                onHit?.Invoke(point);
            }

            if (trace)
            {
                Quaternion rotation = Quaternion.LookRotation(point - position);
                var traceInstance = Lean.Pool.LeanPool.Spawn<Trace>(trace, position, rotation);
                traceInstance.transform.localScale = traceSize;
                traceInstance.Move(position, point);
                onHit?.Invoke(point);
            }

            impactController.Visualize(point, point - position);
        }
    }
}