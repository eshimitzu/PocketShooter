using System;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Core.SchedulerManager;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Weapons
{
    public class MeleeAttackVizualizer : MonoBehaviour, IAttackVisualizer
    {
        [SerializeField]
        private HitImpact hitImpact;
        [SerializeField]
        private string hitSound;
        // TODO: a.dezhurko Get from animation
        [SerializeField]
        private float attackDuration = 0.4f;

        /// <summary>
        /// Launches a bullet toward specified point.
        /// </summary>
        /// <param name="point">The target point.</param>
        /// <param name="onHit">The function to be executed on target hit.</param>
        public void Attack(Vector3 point, Action<Vector3> onHit)
        {
            SchedulerManager.Instance.CallActionWithDelay(gameObject, () => VizualizeAttack(point, onHit), attackDuration);
        }

        private void VizualizeAttack(Vector3 point, Action<Vector3> onHit)
        {
            AudioController.Instance.PostEvent(hitSound, gameObject);
            var impact = Lean.Pool.LeanPool.Spawn(hitImpact, point, Quaternion.LookRotation(transform.forward));
            impact.Play();

            onHit?.Invoke(point);
        }
    }
}
