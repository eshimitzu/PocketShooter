using System;
using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    /// <summary>
    /// Represents settings for weapon view.
    /// </summary>
    /// <seealso cref="UnityEngine.ScriptableObject" />
    [CreateAssetMenu(fileName = "WeaponViewSettings", menuName = "HeyworksMain/Settings/Create Weapons View Settings")]
    public class WeaponViewSettings : ScriptableObject
    {
        [SerializeField]
        private Vector3 defaultAimHelperRotation = Vector3.zero;
        [SerializeField]
        private BodyAimHelperRotation[] bodyAimHelperRotations = null;
        [SerializeField]         private float defaultInputSensetivityMultiplier = 1;

        [Header("Zoom Settings")]
        [SerializeField]
        private float defaultCameraDistance = 2.42f;
        [SerializeField]
        private float defaultCameraFieldOfView = 55;
        [SerializeField]
        private WeaponViewParameters[] weaponViewParameters = null;

        /// <summary>
        /// Gets the aim helper rotation for the specified weapon.
        /// </summary>
        /// <param name="weaponName">Name of the weapon.</param>
        public Vector3 GetAimHelperRotation(WeaponName weaponName)
        {
            foreach (var helperRotation in bodyAimHelperRotations)
            {
                if (helperRotation.Name == weaponName)
                {
                    return helperRotation.Rotation;
                }
            }

            return defaultAimHelperRotation;
        }

        /// <summary>
        /// Gets the weapon view parameters for the specified weapon.
        /// </summary>
        /// <param name="weaponName">Name of the weapon.</param>
        public WeaponViewParameters GetWeaponViewParameters(WeaponName weaponName)
        {
            foreach (var weaponViewSettings in weaponViewParameters)
            {
                if (weaponViewSettings.Name == weaponName)
                {
                    return weaponViewSettings;
                }
            }

            return new WeaponViewParameters
            {
                ZoomCameraDistance = defaultCameraDistance,
                ZoomCameraFieldOfView = defaultCameraFieldOfView,
                ZoomControlSensetivityMultiplier = defaultInputSensetivityMultiplier
            };
        }

        /// <summary>
        /// Gets the default weapon view parameters.
        /// </summary>
        public WeaponViewParameters GetDefaultWeaponViewParameters()
        {
            return new WeaponViewParameters
            {
                ZoomCameraDistance = defaultCameraDistance,
                ZoomCameraFieldOfView = defaultCameraFieldOfView,
                ZoomControlSensetivityMultiplier = defaultInputSensetivityMultiplier
            };
        }

        [Serializable]
        public struct WeaponViewParameters
        {
            public WeaponName Name;
            public float ZoomCameraDistance;
            public float ZoomCameraFieldOfView;
            public float ZoomControlSensetivityMultiplier;
        }

        [Serializable]
        public struct BodyAimHelperRotation
        {
            public WeaponName Name;
            public Vector3 Rotation;
        }
    }
}