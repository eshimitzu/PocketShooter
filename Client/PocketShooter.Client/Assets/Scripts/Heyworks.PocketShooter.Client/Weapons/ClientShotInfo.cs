using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    /// <summary>
    /// Represents information about the shot.
    /// </summary>
    public struct ClientShotInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientShotInfo"/> struct.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="point">The point.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="isCritical">if set to <c>true</c> shot is headshot.</param>
        public ClientShotInfo(NetworkCharacter target, Vector3 point, Vector3 direction, bool isCritical)
        {
            Target = target;
            Point = point;
            Direction = direction;
            IsCritical = isCritical;
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public NetworkCharacter Target { get; private set; }

        /// <summary>
        /// Gets the point.
        /// </summary>
        public Vector3 Point { get; private set; }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public Vector3 Direction { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this shot is headshot.
        /// </summary>
        public bool IsCritical { get; private set; }
    }
}
