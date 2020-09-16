using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class AnimationUtilityRotationController : MonoBehaviour
    {
        [SerializeField]
        private string rotationHorizontalAxisName = "Mouse X";
        [SerializeField]
        private string rotationVerticalAxisName = "Mouse Y";

        [SerializeField]
        private float rotationSensitivityX = 1f;
        [SerializeField]
        private float rotationSensitivityY = 1f;

        private CrossPlatformInputManager.VirtualAxis rotationHorizontalVirtualAxis;
        private CrossPlatformInputManager.VirtualAxis rotationVerticalVirtualAxis;

        private void OnEnable()
        {
            rotationHorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(rotationHorizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(rotationHorizontalVirtualAxis);
            rotationVerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(rotationVerticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(rotationVerticalVirtualAxis);
        }

        private void OnDisable()
        {
            if (CrossPlatformInputManager.AxisExists(rotationHorizontalAxisName))
            {
                CrossPlatformInputManager.UnRegisterVirtualAxis(rotationHorizontalVirtualAxis);
            }

            if (CrossPlatformInputManager.AxisExists(rotationVerticalAxisName))
            {
                CrossPlatformInputManager.UnRegisterVirtualAxis(rotationVerticalVirtualAxis);
            }
        }

        private void Update()
        {
            ProcessRotationInput();
        }

        private void ProcessRotationInput()
        {
            float rotationDeltaX = Input.GetAxis(rotationHorizontalAxisName);
            float rotationDeltaY = Input.GetAxis(rotationVerticalAxisName);

            Vector2 delta = new Vector2(rotationDeltaX, rotationDeltaY);

            UpdateRotationVirtualAxes(delta);
        }

        private void UpdateRotationVirtualAxes(Vector2 value)
        {
            value.x *= rotationSensitivityX;
            value.y *= rotationSensitivityY;

            rotationHorizontalVirtualAxis.Update(value.x);
            rotationVerticalVirtualAxis.Update(value.y);
        }
    }
}