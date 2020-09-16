using UnityEngine;

namespace Heyworks.PocketShooter.Weapons.AimAssistant
{
    /// <summary>
    /// Represents configuration for the aim assistant.
    /// </summary>
    [CreateAssetMenu(
        fileName = "AimAssistantConfiguration",
        menuName = "Heyworks/Settings/Aim Assistant Configuration")]
    public class AimAssistantConfiguration : ScriptableObject
    {
        [SerializeField]
        private float aimAssistActivationRadius = 7;

        [SerializeField]
        private float mainRaycastSensivity = 0.5f;

        [SerializeField]
        private float secondaryRaycastSensivity = 0.75f;

        [SerializeField]
        private float aimAssistSensivityChangeTime = 0.3f;

        [SerializeField]
        private AnimationCurve aimAssistMoveSpeed;

        [SerializeField]
        private float aimAssistActiveInputThreshold = 0.5f;

        /// <summary>
        /// Gets the aim assist activation radius.
        /// </summary>
        public float AimAssistActivationRadius => aimAssistActivationRadius;

        /// <summary>
        /// Gets the main raycast sensivity.
        /// </summary>
        public float MainRaycastSensivity => mainRaycastSensivity;

        /// <summary>
        /// Gets the secondary raycast sensivity.
        /// </summary>
        public float SecondaryRaycastSensivity => secondaryRaycastSensivity;

        /// <summary>
        /// Gets the aim assist sensivity change time.
        /// </summary>
        public float AimAssistSensivityChangeTime => aimAssistSensivityChangeTime;

        public AnimationCurve AimAssistMoveSpeed => aimAssistMoveSpeed;

        public float AimAssistActiveInputThreshold => aimAssistActiveInputThreshold;
    }
}