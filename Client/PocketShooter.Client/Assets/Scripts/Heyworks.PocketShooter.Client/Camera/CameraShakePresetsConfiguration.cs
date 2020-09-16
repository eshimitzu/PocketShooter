using System;
using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;

namespace Heyworks.PocketShooter.Camera
{
    /// <summary>
    /// Container for camera shake presets.
    /// </summary>
    [CreateAssetMenu(fileName = "CameraShakePresetsConfiguration", menuName = "HeyworksMain/Settings/Camera Shake Presets Configuration")]
    public class CameraShakePresetsConfiguration : ScriptableObject
    {
        [Header("Weapon Presets")]
        [SerializeField]
        private float defaultDuration = 1f;
        [SerializeField]
        private float defaultAmount = 1f;
        [SerializeField]
        private CameraShakePreset[] weaponPresets = null;
        [Header("Explosion Presets")]
        [SerializeField]
        private float defaultExplosionDuration = 1f;
        [SerializeField]
        private float defaultExplosionAmount = 1f;

        /// <summary>
        /// Gets the camera shake preset for the specified weapon.
        /// </summary>
        /// <param name="weaponName">Name of the weapon.</param>
        public CameraShakePreset GetCameraShakePreset(WeaponName weaponName)
        {
            foreach (var weaponPreset in weaponPresets)
            {
                if (weaponPreset.WeaponName == weaponName)
                {
                    return weaponPreset;
                }
            }

            return new CameraShakePreset
            {
                AnimationDuration = 0.001f,
                AnimationAmount = 0,
                AnimationCurve = AnimationCurve.Linear(0, 0, 1, 0),
                AddRandomization = true,
                ShakeDuration = defaultDuration,
                ShakeAmount = defaultAmount,
            };
        }

        /// <summary>
        /// Gets the camera shake explosion preset.
        /// </summary>
        public CameraShakePreset GetCameraShakeExplosionPreset()
        {
            return new CameraShakePreset
            {
                AnimationDuration = 0.001f,
                AnimationAmount = 0,
                AnimationCurve = AnimationCurve.Linear(0, 0, 1, 0),
                AddRandomization = true,
                ShakeDuration = defaultExplosionDuration,
                ShakeAmount = defaultExplosionAmount,
            };
        }

        /// <summary>
        /// Represents canera shake preset.
        /// </summary>
        [Serializable]
        public class CameraShakePreset
        {
#pragma warning disable SA1401, SA1600
            public WeaponName WeaponName;

            public float AnimationDuration;
            public float AnimationAmount;
            public AnimationCurve AnimationCurve;

            public bool AddRandomization;
            public float ShakeDuration;
            public float ShakeAmount;

            public float Duration => Mathf.Max(AnimationDuration, ShakeDuration);
#pragma warning restore SA1401, SA1600

            /// <summary>
            /// Evaluates a rotation amout at specified time.
            /// </summary>
            /// <param name="time">The time.</param>
            public Vector3 Evaluate(float time)
            {
                var rotationAxis = -Vector3.right;
                var shakeRotation = AnimationCurve.Evaluate(ZeroClamp(time, AnimationDuration) / AnimationDuration) * AnimationAmount * rotationAxis;

                if (AddRandomization)
                {
                    var shakeAmount = ShakeAmount * (ZeroClamp(time, ShakeDuration) / ShakeDuration);
                    var rotationAmount = UnityEngine.Random.insideUnitSphere * shakeAmount;

                    shakeRotation += rotationAmount;
                }

                return shakeRotation;
            }

            private float ZeroClamp(float value, float max)
            {
                if (value > max)
                {
                    return 0f;
                }

                return value;
            }
        }
    }
}
