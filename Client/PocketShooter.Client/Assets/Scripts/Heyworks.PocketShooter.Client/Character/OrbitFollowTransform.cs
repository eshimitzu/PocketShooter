using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents class for simulating camera behavior on the remote characters.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class OrbitFollowTransform : MonoBehaviour
    {
        [SerializeField]
        private float followingSharpness = 30f;

        [SerializeField]
        private float defaultDistance = 2.42f;

        private Vector3 currentFollowPosition;
        private Transform followTransform;

        /// <summary>
        /// Setups the follow transform.
        /// </summary>
        /// <param name="follow">The follow transform.</param>
        public void Setup(Transform follow)
        {
            followTransform = follow;
        }

        /// <summary>
        /// Updates the with rotation.
        /// </summary>
        /// <param name="targetRotation">The target rotation.</param>
        public void UpdateWithRotation(Quaternion targetRotation)
        {
            float deltaTime = Time.deltaTime;

            // Find the smoothed follow position
            currentFollowPosition = Vector3.Lerp(currentFollowPosition, followTransform.position, 1f - Mathf.Exp(-followingSharpness * deltaTime));

            // Apply rotation
            transform.rotation = targetRotation;
            Vector3 targetPosition = currentFollowPosition - ((targetRotation * Vector3.forward) * defaultDistance);
            transform.position = targetPosition;
        }
    }
}