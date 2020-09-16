using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Heyworks.PocketShooter.Weapons;

namespace Heyworks.PocketShooter.Camera
{
    /// <summary>
    /// Represents camera controller for smooth follow.
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        private const float NotOccludedDistance = -1f;

        [Header("Following")]
        [SerializeField]
        private float followingSharpness = 30f;
        [Header("Distance")]
        [SerializeField]
        private float defaultDistance = 6f;

        [Header("Rotation")]
        [Range(-90f, 90f)]
        [SerializeField]
        private float defaultVerticalAngle = 20f;
        [Range(-90f, 90f)]
        [SerializeField]
        private float minVerticalAngle = -80f;
        [Range(-90f, 90f)]
        [SerializeField]
        private float maxVerticalAngle = 80f;
        [SerializeField]
        private float rotationSpeed = 10f;
        [SerializeField]
        private float rotationSharpness = 30f;

        [Header("Obstruction")]
        [SerializeField]
        private float obstructionSharpnessForward = 10000f;
        [SerializeField]
        private float obstructionSharpnessBackward = 10f;
        [SerializeField]
        private float obstructionDistanceStep = 0.01f;
        [SerializeField]
        private int obstructionIterations = 10;
        [SerializeField]
        private LayerMask layerMask = 0;
        [SerializeField]
        private float followPointOffsetLerp = 0.2f;
        [SerializeField]
        private float minFollowPointDistanceToWall = 0.63f;
        [SerializeField]
        private PostProcessVolume postProcessVolume;
        [SerializeField]
        private PostProcessLayer postProcessLayer;

        private Vector3 planarDirection;
        private float currentDistance;
        private float targetVerticalAngle;
        private Vector3 currentFollowPosition;
        private Transform followedTransform;
        private UnityEngine.Camera orbitCamera;

        private float cachedDefaultDistance;
        private float lastTargetFollowPointXOffset;

        /// <summary>
        /// Gets the transform of camera.
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// Gets the camera shake component.
        /// </summary>
        public CameraShake CameraShake { get; private set; }

        /// <summary>
        /// Tells camera to follow transform, the transform that the camera will orbit around.
        /// </summary>
        /// <param name="transformToFollow">The transform to set.</param>
        public void SetFollowTransform(Transform transformToFollow)
        {
            followedTransform = transformToFollow;
            planarDirection = followedTransform.forward;
            currentFollowPosition = followedTransform.position;

            lastTargetFollowPointXOffset = GetDefaultFollowPointXOffset();
        }

        public void EnableBlur(bool enable)
        {
            if (postProcessVolume && postProcessLayer)
            {
                postProcessVolume.enabled = enable;
                postProcessLayer.enabled = enable;
            }
        }

        /// <summary>
        /// Applies inputs to the camera.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="rotationInput">The rotation input.</param>
        public void UpdateWithInput(float deltaTime, Vector3 rotationInput)
        {
            if (followedTransform)
            {
                // Process rotation input
                var up = followedTransform.up;
                Quaternion rotationFromInput = Quaternion.Euler(up * (rotationInput.x * rotationSpeed));
                planarDirection = rotationFromInput * planarDirection;
                planarDirection = Vector3.Cross(up, Vector3.Cross(planarDirection, up));
                targetVerticalAngle -= rotationInput.y * rotationSpeed;
                targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);

                var nextFollowPosition = CalculateFollowPosition();

                // Find the smoothed follow position
                currentFollowPosition = Vector3.Lerp(currentFollowPosition, nextFollowPosition, 1f - Mathf.Exp(-followingSharpness * deltaTime));

                // Calculate smoothed rotation
                Quaternion planarRot = Quaternion.LookRotation(planarDirection, up);
                Quaternion verticalRot = Quaternion.Euler(targetVerticalAngle, 0, 0);
                Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-rotationSharpness * deltaTime));

                // Apply rotation
                Transform.rotation = targetRotation;

                // Handle obstructions
                HandleObstructions(deltaTime);

                // Find the smoothed camera orbit position
                Vector3 targetPosition = currentFollowPosition - ((targetRotation * Vector3.forward) * currentDistance);

                // Apply position
                Transform.position = targetPosition;
            }
        }

        public void UpdateCameraSetings(WeaponViewSettings.WeaponViewParameters weaponViewParameters)
        {
            if (orbitCamera)
            {
                orbitCamera.fieldOfView = weaponViewParameters.ZoomCameraFieldOfView;
                defaultDistance = weaponViewParameters.ZoomCameraDistance;
            }
        }

        private void OnValidate() =>
            defaultVerticalAngle = Mathf.Clamp(defaultVerticalAngle, minVerticalAngle, maxVerticalAngle);

        private void Awake()
        {
            Transform = transform;
            CameraShake = GetComponent<CameraShake>();
            orbitCamera = GetComponent<UnityEngine.Camera>();
            currentDistance = defaultDistance;
            targetVerticalAngle = 0f;
            planarDirection = Vector3.forward;
        }

        private float GetDefaultFollowPointXOffset() =>
            followedTransform.localPosition.x;

        /// <summary>
        /// Draw lines in the editor view to make it easier to visualize.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="clipPlanePoints">Clip plane points.</param>
        private void DrawClipPlane(Vector3 from, Vector3 to, ClipPlanePoints clipPlanePoints)
        {
            Debug.DrawLine(from, to + (transform.forward * -orbitCamera.nearClipPlane), Color.red);
            Debug.DrawLine(from, clipPlanePoints.UpperLeft);
            Debug.DrawLine(from, clipPlanePoints.LowerLeft);
            Debug.DrawLine(from, clipPlanePoints.UpperRight);
            Debug.DrawLine(from, clipPlanePoints.LowerRight);

            Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
            Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
            Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
            Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);
        }

        private Vector3 CalculateFollowPosition()
        {
            var followPointOffset = GetDefaultFollowPointXOffset();
            var followPointDirSign = Mathf.Sign(followPointOffset);

            var center = followedTransform.position - followedTransform.right * followPointOffset;
            var testFrom = center - followedTransform.right * minFollowPointDistanceToWall * followPointDirSign;
            var testTo = followedTransform.position + followedTransform.right * minFollowPointDistanceToWall * followPointDirSign;

            var validDistance = NotOccludedDistance;
            LineCast(testFrom, testTo, ref validDistance);

            var targetFollowPointXOffset = (validDistance == NotOccludedDistance) ?
                followPointOffset :
                followPointDirSign * (validDistance - 2 * minFollowPointDistanceToWall);

            lastTargetFollowPointXOffset = Mathf.Lerp(lastTargetFollowPointXOffset, targetFollowPointXOffset, followPointOffsetLerp);

            var validPos = center + followedTransform.right * lastTargetFollowPointXOffset;

            return validPos;
        }

        private void HandleObstructions(float deltaTime)
        {
            var currentTargetDistance = defaultDistance;
            var iterations = 0;
            bool isOccluded = true;

            do
            {
                iterations++;

                var desiredPosition = currentFollowPosition - ((Transform.rotation * Vector3.forward) * currentTargetDistance);
                var occludedDistance = CheckClipPlaneCameraPoints(currentFollowPosition, desiredPosition);

                if (Math.Abs(occludedDistance - NotOccludedDistance) < 0.001)
                {
                    isOccluded = false;
                }
                else
                {
                    currentTargetDistance = occludedDistance - obstructionDistanceStep;
                }
            }
            while (iterations < obstructionIterations && isOccluded);

            var sharpness = currentDistance > currentTargetDistance
                            ? obstructionSharpnessForward
                            : obstructionSharpnessBackward;

            currentDistance = Mathf.Lerp(currentDistance, currentTargetDistance, 1 - Mathf.Exp(-sharpness * deltaTime));
        }

        private float CheckClipPlaneCameraPoints(Vector3 from, Vector3 to)
        {
            var nearDistance = NotOccludedDistance;
            ClipPlanePoints clipPlanePoints = ClipPlaneAtNear(to);

            DrawClipPlane(from, to, clipPlanePoints);

            LineCast(from, clipPlanePoints.UpperLeft, ref nearDistance);
            LineCast(from, clipPlanePoints.LowerLeft, ref nearDistance);
            LineCast(from, clipPlanePoints.UpperRight, ref nearDistance);
            LineCast(from, clipPlanePoints.LowerRight, ref nearDistance);
            LineCast(from, to + (transform.forward * -orbitCamera.nearClipPlane), ref nearDistance);

            return nearDistance;
        }

        private void LineCast(Vector3 start, Vector3 end, ref float distance)
        {
            if (Physics.Linecast(start, end, out var hitInfo, layerMask))
            {
                if (hitInfo.distance < distance || distance == NotOccludedDistance)
                {
                    distance = hitInfo.distance;
                }
            }
        }

        private ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
        {
            ClipPlanePoints clipPlanePoints;

            var halfFov = (orbitCamera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = orbitCamera.aspect;
            var distance = orbitCamera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFov);
            var width = height * aspect;
            var right = Transform.right;
            var up = Transform.up;
            var forward = Transform.forward;

            clipPlanePoints.LowerRight = pos + (right * width);
            clipPlanePoints.LowerRight -= up * height;
            clipPlanePoints.LowerRight += forward * distance;

            clipPlanePoints.LowerLeft = pos - (right * width);
            clipPlanePoints.LowerLeft -= up * height;
            clipPlanePoints.LowerLeft += forward * distance;

            clipPlanePoints.UpperRight = pos + (right * width);
            clipPlanePoints.UpperRight += up * height;
            clipPlanePoints.UpperRight += forward * distance;

            clipPlanePoints.UpperLeft = pos - (right * width);
            clipPlanePoints.UpperLeft += up * height;
            clipPlanePoints.UpperLeft += forward * distance;

            return clipPlanePoints;
        }

        private struct ClipPlanePoints
        {
            public Vector3 UpperLeft;
            public Vector3 UpperRight;
            public Vector3 LowerLeft;
            public Vector3 LowerRight;
        }
    }
}