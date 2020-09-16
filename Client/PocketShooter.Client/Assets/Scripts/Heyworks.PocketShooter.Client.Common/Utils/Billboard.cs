using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Represents the billboard.
    /// Makes the object which it is attached to align itself with the camera.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        private Camera referenceCamera;

        [SerializeField]
        private bool reverseFace = false;
        [SerializeField]
        private Axis axis = Axis.Up;

        private void Awake()
        {
            if (!referenceCamera)
            {
                referenceCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            Quaternion rot = referenceCamera.transform.rotation;
            Vector3 targetPos = transform.position + rot * (reverseFace ? Vector3.forward : Vector3.back);
            Vector3 targetOrientation = rot * GetAxis(axis);
            transform.LookAt(targetPos, targetOrientation);
        }

        private Vector3 GetAxis(Axis refAxis)
        {
            switch (refAxis)
            {
                case Axis.Down:
                    return Vector3.down;
                case Axis.Forward:
                    return Vector3.forward;
                case Axis.Back:
                    return Vector3.back;
                case Axis.Left:
                    return Vector3.left;
                case Axis.Right:
                    return Vector3.right;
            }

            return Vector3.up;
        }
    }
}
