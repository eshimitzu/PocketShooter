using System.Collections;
using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;

namespace Heyworks.PocketShooter.Camera
{
    /// <summary>
    /// Applies random shake to the Transform.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        [SerializeField]
        private CameraShakePresetsConfiguration shakePresetsConfiguration = null;

        private bool isRunning;

        /// <summary>
        /// Shakes the camera for the specified weapon.
        /// </summary>
        /// <param name="weaponName">Name of the weapon.</param>
        public void ShakeCamera(WeaponName weaponName)
        {
            if (!isRunning)
            {
                StartCoroutine(Shake(shakePresetsConfiguration.GetCameraShakePreset(weaponName)));
            }
        }

        /// <summary>
        /// Shakes the camera.
        /// </summary>
        [ContextMenu("Shake Camera")]
        public void ShakeCamera()
        {
            if (!isRunning)
            {
                StartCoroutine(Shake(shakePresetsConfiguration.GetCameraShakeExplosionPreset()));
            }
        }

        private IEnumerator Shake(CameraShakePresetsConfiguration.CameraShakePreset shakePreset)
        {
            isRunning = true;
            var duration = 0f;

            while (duration < shakePreset.Duration)
            {
                duration += Time.deltaTime;
                transform.localRotation *= Quaternion.Euler(shakePreset.Evaluate(duration));

                yield return null;
            }

            isRunning = false;
        }
    }
}