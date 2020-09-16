using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class CausedDamageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the position.
        /// </summary>
        public Vector3 Position
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the damage.
        /// </summary>
        public float Damage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether damage is critical.
        /// </summary>
        public bool IsCritical
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CausedDamageEventArgs"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="damage">The damage.</param>
        /// <param name="isCritical">Value indicating whether damage is critical.</param>
        public CausedDamageEventArgs(Vector3 position, float damage, bool isCritical)
        {
            Position = position;
            Damage = damage;
            IsCritical = isCritical;
        }
    }
}