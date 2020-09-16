using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Weapons;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Class which responsible for rotation of the trooper's body toward an attack target.
    /// </summary>
    public class BodyAimRotationHelper : MonoBehaviour
    {
        private const string AimingToggle = "Aiming";

        [SerializeField]
        private WeaponViewSettings weaponViewSettings;
        [SerializeField]
        private bool isLocalPlayer;

        private bool isReady;
        private bool isDisabled;
        private Transform hipsBone;
        private int aimingToggleId;
        private Vector3 initialHelperRotation;
        private Animator animator;
        private Transform helperTransform;

        private Transform eyeTransform;

        public void Setup(WeaponName weaponName, Transform eye, Animator animator)
        {
            this.animator = animator;

            eyeTransform = eye;
            initialHelperRotation = weaponViewSettings.GetAimHelperRotation(weaponName);
        }

        public void SetRotation(Quaternion rotation)
        {
            if (isReady)
            {
                helperTransform.localRotation = rotation;
            }
        }

        public Quaternion GetRotation()
        {
            if (isReady)
            {
                return helperTransform.localRotation;
            }

            return Quaternion.identity;
        }

        private void Start()
        {
            aimingToggleId = Animator.StringToHash(AimingToggle);

            hipsBone = animator.transform.Find("Trooper_Root/Trooper_Hips");
            var spine2 = animator.transform.Find("Trooper_Root/Trooper_Hips/Trooper_Spine1/Trooper_Spine2");
            var spine3 = animator.transform.Find("Trooper_Root/Trooper_Hips/Trooper_Spine1/Trooper_Spine2/Trooper_Spine3");

            helperTransform = new GameObject("BodyAimRotationHelper").transform;
            helperTransform.SetParent(spine2, false);
            spine3.SetParent(helperTransform, true);

            isReady = true;
        }

        private void Update()
        {
            if (isLocalPlayer && isReady && animator.GetBool(aimingToggleId))
            {
                var eyeForward = eyeTransform.forward;
                var eyeRight = eyeTransform.right;
                var hipsForward = hipsBone.forward;

                var cameraForwardXZProjection = Vector3.ProjectOnPlane(eyeForward, Vector3.up);
                var hipsXZProjection = Vector3.ProjectOnPlane(hipsForward, Vector3.up);
                var leftRightRotationAngle = Vector3.Angle(cameraForwardXZProjection, hipsXZProjection);
                leftRightRotationAngle *= Mathf.Sign(Vector3.Cross(cameraForwardXZProjection, hipsXZProjection).y);

                var cameraForwardYZProjection = Vector3.ProjectOnPlane(eyeForward, eyeRight);
                var hipsYZProjection = Vector3.ProjectOnPlane(hipsForward, eyeRight);
                var p1 = new Vector3(0, cameraForwardYZProjection.y, Mathf.Sqrt(1 - (cameraForwardYZProjection.y * cameraForwardYZProjection.y)));
                var p2 = new Vector3(0, hipsYZProjection.y, Mathf.Sqrt(1 - (hipsYZProjection.y * hipsYZProjection.y)));
                var rotationAngleUpDown = Vector3.Angle(p1, p2);
                rotationAngleUpDown *= Mathf.Sign(Vector3.Cross(p1, p2).x);

                helperTransform.localRotation =
                    Quaternion.Slerp(helperTransform.localRotation, Quaternion.Euler(0, 0, rotationAngleUpDown - initialHelperRotation.z), 0.6f);
                isDisabled = false;
            }
            else if (!isDisabled && !animator.GetBool(aimingToggleId))
            {
                isDisabled = true;
                helperTransform.localRotation = Quaternion.identity;
            }
        }
    }
}
