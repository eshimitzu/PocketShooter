using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public interface IAttackVisualizer
    {
        /// <summary>
        /// Launches a bullet toward specified point.
        /// </summary>
        /// <param name="point">The target point.</param>
        /// <param name="onHit">The function to be executed on target hit.</param>
        void Attack(Vector3 point, Action<Vector3> onHit);
    }
}