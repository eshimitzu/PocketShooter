using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents settings for character controller.
    /// </summary>
    /// <seealso cref="UnityEngine.ScriptableObject" />
    [CreateAssetMenu(fileName = "CharacterControllerSettings", menuName = "HeyworksMain/Settings/Create Character Controller Settings")]
    public class CharacterControllerSettings : ScriptableObject
    {
        [Header("Stable Movement")]
        [SerializeField]
        private float maxStableMoveSpeed = 3f;
        [SerializeField]
        private float maxRightLeftStableMoveCoefficient = 0.8f;
        [SerializeField]
        private float maxBackwardStableMoveCoefficient = 0.5f;
        [SerializeField]
        private float stableMovementSharpness = 100f;
        [SerializeField]
        private float orientationSharpness = 100f;

        [Header("Air Movement")]
        [SerializeField]
        private float maxAirMoveSpeed = 5f;
        [SerializeField]
        private float airAccelerationSpeed = 2f;
        [SerializeField]
        private float drag = 0.1f;

        [Header("Misc")]
        [SerializeField]
        private Vector3 gravity = new Vector3(0, -90f, 0);

        [SerializeField]
        private float fallMultiplier = 1f;

        /// <summary>
        /// Gets the maximum stable move speed.
        /// </summary>
        public float MaxStableMoveSpeed
        {
            get { return maxStableMoveSpeed; }
        }

        /// <summary>
        /// Gets the stable movement sharpness.
        /// </summary>
        public float StableMovementSharpness
        {
            get { return stableMovementSharpness; }
        }

        /// <summary>
        /// Gets the orientation sharpness.
        /// </summary>
        public float OrientationSharpness
        {
            get { return orientationSharpness; }
        }

        /// <summary>
        /// Gets the maximum air move speed.
        /// </summary>
        public float MaxAirMoveSpeed
        {
            get { return maxAirMoveSpeed; }
        }

        /// <summary>
        /// Gets the air acceleration speed.
        /// </summary>
        public float AirAccelerationSpeed
        {
            get { return airAccelerationSpeed; }
        }

        /// <summary>
        /// Gets the drag.
        /// </summary>
        public float Drag
        {
            get { return drag; }
        }

        /// <summary>
        /// Gets the gravity.
        /// </summary>
        public Vector3 Gravity
        {
            get { return gravity; }
        }

        /// <summary>
        /// Gets the gravity.
        /// </summary>
        public float FallMultiplier
        {
            get { return fallMultiplier; }
        }

        /// <summary>
        /// Gets the input scale coefficient.
        /// </summary>
        /// <param name="direction">The move direction.</param>
        public float GetSpeedScaleCoefficient(Vector3 direction)
        {
            var dir = new Vector3(direction.x, 0, direction.z);

            if (dir.z >= 0)
            {
                var angle = Vector3.Angle(Vector3.forward, dir);
                return Mathf.Lerp(1, maxRightLeftStableMoveCoefficient, angle / 90f);
            }
            else
            {
                var angle = Vector3.Angle(Vector3.back, dir);
                return Mathf.Lerp(maxBackwardStableMoveCoefficient, maxRightLeftStableMoveCoefficient, angle / 90f);
            }
        }
    }
}
